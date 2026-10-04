using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Billing;
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Memberships;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

public class InvoiceService(IInvoiceRepository invoiceRepository,
                            IInvoiceLineRepository lineRepository,
                            IUserAddressRepository addressRepository,
                            IUserContactReader contactReader,
                            IMemberDiscountReader memberDiscountReader,
                            IServiceCatalogReader catalogReader,
                            IEmailOutboxWriter<ICommerceOutboxModule> outboxWriter,
                            IEmailTemplate<InvoiceEmailModel> emailTemplate,
                            IPushNotificationSender pushSender,
                            InvoiceBusinessInfo business,
    ITenantTimeZoneReader timeZoneReader,
                            ILogger<InvoiceService> logger)
    : IInvoiceService, IServiceInvoiceIssuer
{
    private const string Currency = "USD";

    public async Task<Invoice> IssueForPaidOrderAsync(Order order, IReadOnlyList<OrderItem> items, CancellationToken cancellationToken = default)
    {
        if (await FindForSourceAsync(InvoiceSources.Order, order.Id, cancellationToken) is { } existing)
        {
            return existing;
        }

        var contact = await contactReader.GetContactAsync(order.UserId, cancellationToken);

        var billingAddress = await addressRepository.GetAsync(a => a.Id == order.InvoiceAddressId, cancellationToken: cancellationToken);

        var titles = await catalogReader.GetManyAsync(items.Select(i => i.ServiceId).Distinct(), cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var billTo = new InvoiceBillTo(contact?.FullName is { Length: > 0 } name ? name : "Customer", contact?.Email,
            billingAddress is null ? null : $"{billingAddress.AddressLine}, {billingAddress.City}, {billingAddress.State} {billingAddress.Zipcode}");

        var charges = items.Select(item => new InvoiceCharge(titles.TryGetValue(item.ServiceId, out var entry) ? entry.Title : "Item", item.Quantity, item.UnitPrice,
            item.TaxRate)).ToList();

        var (invoice, lines) = Invoice.Issue(InvoiceNumberGenerator.Generate(now), order.UserId, InvoiceSources.Order, order.Id, order.OrderNumber,
            Currency, billTo, charges, discount: null, now, dueAt: null);

        invoice.MarkPaid(InvoicePaymentMethods.Card, order.PaymentProvider, order.ProviderPaymentId, now);

        await SaveAsync(invoice, lines, cancellationToken);

        return invoice;
    }

    public async Task<Guid?> IssueForCompletedVisitAsync(ServiceInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        var billable = request.Lines.Where(l => l.Quantity > 0 && !string.IsNullOrWhiteSpace(l.Description)).ToList();

        if (billable.Count == 0)
        {
            return null;
        }

        if (await FindForSourceAsync(InvoiceSources.Appointment, request.AppointmentId, cancellationToken) is { } existing)
        {
            return existing.Id;
        }

        var contact = await contactReader.GetContactAsync(request.CustomerUserId, cancellationToken);

        var discount = await memberDiscountReader.GetActiveDiscountAsync(request.CustomerUserId, cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var billTo = new InvoiceBillTo(contact?.FullName is { Length: > 0 } name ? name : "Customer", contact?.Email, request.ServiceAddress);

        var charges = billable.Select(line => new InvoiceCharge(line.Description, line.Quantity, Math.Max(0, line.UnitPrice), Math.Clamp(line.TaxRate, 0, 100))).ToList();

        var memberDiscount = discount is null ? null : new InvoiceDiscount(discount.Percent, $"Member discount ({discount.PlanName}, {discount.Percent:0.##}%)");

        var (invoice, lines) = Invoice.Issue(InvoiceNumberGenerator.Generate(now), request.CustomerUserId, InvoiceSources.Appointment, request.AppointmentId,
            $"Service visit: {request.ServiceTitle}", Currency, billTo, charges, memberDiscount, now, now.AddDays(business.PaymentTermsDays));

        await SaveAsync(invoice, lines, cancellationToken);

        if (invoice.Total <= 0)
        {
            await MarkPaidAsync(invoice, InvoicePaymentMethods.Covered, null, null, cancellationToken);

            return invoice.Id;
        }

        var response = InvoiceResponse.FromDomain(invoice, lines);

        await EmailAsync(InvoiceEmailKind.Issued, response, invoice.TenantId, cancellationToken);

        await PushAsync(invoice, "New invoice", $"{invoice.InvoiceNumber} for {invoice.Title}: {Money(invoice.Total)} due.", cancellationToken);

        return invoice.Id;
    }

    public async Task MarkPaidAsync(Invoice invoice, string paymentMethod, string? paymentProvider, string? providerPaymentId, CancellationToken cancellationToken = default)
    {
        invoice.MarkPaid(paymentMethod, paymentProvider, providerPaymentId, DateTimeOffset.UtcNow);

        _ = await invoiceRepository.UpdateAsync(invoice, cancellationToken: cancellationToken);

        var response = await ToResponseAsync(invoice, cancellationToken);

        await EmailAsync(InvoiceEmailKind.Receipt, response, invoice.TenantId, cancellationToken);

        await PushAsync(invoice, "Payment received", $"Thanks! {invoice.InvoiceNumber} is paid.", cancellationToken);
    }

    public async Task VoidAsync(Invoice invoice, string? reason, CancellationToken cancellationToken = default)
    {
        invoice.Void(reason);

        _ = await invoiceRepository.UpdateAsync(invoice, cancellationToken: cancellationToken);

        var response = await ToResponseAsync(invoice, cancellationToken);

        await EmailAsync(InvoiceEmailKind.Voided, response, invoice.TenantId, cancellationToken);

        await PushAsync(invoice, "Invoice cancelled", $"{invoice.InvoiceNumber} was cancelled. There's nothing to pay for it.", cancellationToken);
    }

    public async Task<InvoiceResponse> ToResponseAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var lines = await lineRepository.GetAllAsync(l => l.InvoiceId == invoice.Id, orderBy: q => q.OrderBy(l => l.SortOrder), cancellationToken: cancellationToken);

        return InvoiceResponse.FromDomain(invoice, lines);
    }

    public string? ViewUrl(Guid invoiceId) =>
        string.IsNullOrWhiteSpace(business.ClientOrigin) || business.ClientOrigin.StartsWith("REPLACE", StringComparison.Ordinal)
            ? null
            : $"{business.ClientOrigin.TrimEnd('/')}/user/invoices/{invoiceId}";

    private async Task<Invoice?> FindForSourceAsync(string sourceType, Guid sourceId, CancellationToken cancellationToken) =>
        await invoiceRepository.GetAsync(i => i.SourceType == sourceType && i.SourceId == sourceId && i.Status != InvoiceStatuses.Void, cancellationToken: cancellationToken);

    private async Task SaveAsync(Invoice invoice, IReadOnlyList<InvoiceLine> lines, CancellationToken cancellationToken)
    {
        _ = await invoiceRepository.AddAsync(invoice, cancellationToken: cancellationToken);

        foreach (var line in lines)
        {
            _ = await lineRepository.AddAsync(line, cancellationToken: cancellationToken);
        }
    }

    private async Task EmailAsync(InvoiceEmailKind kind, InvoiceResponse invoice, Guid tenantId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(invoice.BillToEmail))
        {
            return;
        }

        try
        {
            var model = new InvoiceEmailModel(kind, invoice, business.BusinessName, ViewUrl(invoice.Id), await timeZoneReader.GetAsync(tenantId, cancellationToken));

            await outboxWriter.EnqueueAsync(invoice.BillToEmail, emailTemplate.Subject(model), emailTemplate.Build(model), sourceReference: $"Invoice:{invoice.Id}:{kind}", cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Invoice {InvoiceNumber}: queuing the {Kind} email failed.", invoice.InvoiceNumber, kind);
        }
    }

    private async Task PushAsync(Invoice invoice, string title, string body, CancellationToken cancellationToken)
    {
        try
        {
            await pushSender.SendToUserAsync(invoice.UserId, title, body, new Dictionary<string, string> { ["invoiceId"] = invoice.Id.ToString() }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Invoice {InvoiceNumber}: sending '{Title}' failed.", invoice.InvoiceNumber, title);
        }
    }

    private static string Money(decimal amount) => amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
}
