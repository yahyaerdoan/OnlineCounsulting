using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Billing;
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Memberships;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IInvoiceLineRepository lineRepository,
    IUserAddressRepository addressRepository,
    IUserContactReader contactReader,
    IMemberDiscountReader memberDiscountReader,
    IServiceCatalogReader catalogReader,
    IEmailOutboxWriter<ICommerceOutboxModule> outboxWriter,
    IEmailTemplate<InvoiceEmailModel> emailTemplate,
    IPushNotificationSender pushSender,
    InvoiceBusinessInfo business,
    ILogger<InvoiceService> logger) : IInvoiceService, IServiceInvoiceIssuer
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

        var invoice = new Invoice
        {
            InvoiceNumber = InvoiceCalculator.NewNumber(now),
            UserId = order.UserId,
            SourceType = InvoiceSources.Order,
            SourceId = order.Id,
            Status = InvoiceStatuses.Paid,
            Currency = Currency,
            Title = $"Order {order.OrderNumber}",
            BillToName = contact?.FullName is { Length: > 0 } name ? name : "Customer",
            BillToEmail = contact?.Email,
            BillToAddress = billingAddress is null ? null : $"{billingAddress.AddressLine}, {billingAddress.City}, {billingAddress.State} {billingAddress.Zipcode}",
            IssuedAt = now,
            PaidAt = now,
            PaymentMethod = InvoicePaymentMethods.Card,
            PaymentProvider = order.PaymentProvider,
            ProviderPaymentId = order.ProviderPaymentId,
        };

        var lines = items.Select((item, index) => new InvoiceLine
        {
            InvoiceId = invoice.Id,
            SortOrder = index,
            Description = titles.TryGetValue(item.ServiceId, out var entry) ? entry.Title : "Item",
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TaxRate = item.TaxRate,
        }).ToList();

        await SaveAsync(invoice, lines, discountPercent: 0);
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

        var invoice = new Invoice
        {
            InvoiceNumber = InvoiceCalculator.NewNumber(now),
            UserId = request.CustomerUserId,
            SourceType = InvoiceSources.Appointment,
            SourceId = request.AppointmentId,
            Status = InvoiceStatuses.Open,
            Currency = Currency,
            Title = $"Service visit: {request.ServiceTitle}",
            BillToName = contact?.FullName is { Length: > 0 } name ? name : "Customer",
            BillToEmail = contact?.Email,
            BillToAddress = request.ServiceAddress,
            DiscountLabel = discount is null ? null : $"Member discount ({discount.PlanName}, {discount.Percent:0.##}%)",
            IssuedAt = now,
            DueAt = now.AddDays(business.PaymentTermsDays),
        };

        var lines = billable.Select((line, index) => new InvoiceLine
        {
            InvoiceId = invoice.Id,
            SortOrder = index,
            Description = line.Description.Trim(),
            Quantity = line.Quantity,
            UnitPrice = Math.Max(0, line.UnitPrice),
            TaxRate = Math.Clamp(line.TaxRate, 0, 100),
        }).ToList();

        await SaveAsync(invoice, lines, discount?.Percent ?? 0);

        if (invoice.Total <= 0)
        {
            await MarkPaidAsync(invoice, InvoicePaymentMethods.Covered, null, null, cancellationToken);
            return invoice.Id;
        }

        var response = InvoiceResponse.FromDomain(invoice, lines);
        await EmailAsync(InvoiceEmailKind.Issued, response, cancellationToken);
        await PushAsync(invoice, "New invoice", $"{invoice.InvoiceNumber} for {invoice.Title.ToLowerInvariant()}: {Money(invoice.Total)} due.", cancellationToken);

        return invoice.Id;
    }

    public async Task MarkPaidAsync(Invoice invoice, string paymentMethod, string? paymentProvider, string? providerPaymentId, CancellationToken cancellationToken = default)
    {
        invoice.Status = InvoiceStatuses.Paid;
        invoice.PaidAt = DateTimeOffset.UtcNow;
        invoice.PaymentMethod = paymentMethod;
        invoice.PaymentProvider = paymentProvider ?? invoice.PaymentProvider;
        invoice.ProviderPaymentId = providerPaymentId ?? invoice.ProviderPaymentId;
        _ = await invoiceRepository.UpdateAsync(invoice);

        var response = await ToResponseAsync(invoice, cancellationToken);
        await EmailAsync(InvoiceEmailKind.Receipt, response, cancellationToken);
        await PushAsync(invoice, "Payment received", $"Thanks! {invoice.InvoiceNumber} is paid.", cancellationToken);
    }

    public async Task VoidAsync(Invoice invoice, string? reason, CancellationToken cancellationToken = default)
    {
        invoice.Status = InvoiceStatuses.Void;
        invoice.VoidReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        _ = await invoiceRepository.UpdateAsync(invoice);

        var response = await ToResponseAsync(invoice, cancellationToken);
        await EmailAsync(InvoiceEmailKind.Voided, response, cancellationToken);
        await PushAsync(invoice, "Invoice cancelled", $"{invoice.InvoiceNumber} was cancelled. There's nothing to pay for it.", cancellationToken);
    }

    public async Task<InvoiceResponse> ToResponseAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var lines = await lineRepository.GetListAsync(l => l.InvoiceId == invoice.Id, orderBy: q => q.OrderBy(l => l.SortOrder),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        return InvoiceResponse.FromDomain(invoice, lines.Items);
    }

    public string? ViewUrl(Guid invoiceId) =>
        string.IsNullOrWhiteSpace(business.ClientOrigin) || business.ClientOrigin.StartsWith("REPLACE", StringComparison.Ordinal)
            ? null
            : $"{business.ClientOrigin.TrimEnd('/')}/user/invoices/{invoiceId}";

    private async Task<Invoice?> FindForSourceAsync(string sourceType, Guid sourceId, CancellationToken cancellationToken) =>
        await invoiceRepository.GetAsync(i => i.SourceType == sourceType && i.SourceId == sourceId && i.Status != InvoiceStatuses.Void, cancellationToken: cancellationToken);

    private async Task SaveAsync(Invoice invoice, List<InvoiceLine> lines, decimal discountPercent)
    {
        foreach (var line in lines)
        {
            InvoiceCalculator.ApplyLine(line, discountPercent);
        }

        InvoiceCalculator.ApplyTotals(invoice, lines);
        _ = await invoiceRepository.AddAsync(invoice);
        foreach (var line in lines)
        {
            _ = await lineRepository.AddAsync(line);
        }
    }

    private async Task EmailAsync(InvoiceEmailKind kind, InvoiceResponse invoice, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(invoice.BillToEmail))
        {
            return;
        }

        try
        {
            var model = new InvoiceEmailModel(kind, invoice, business.BusinessName, ViewUrl(invoice.Id));
            await outboxWriter.EnqueueAsync(invoice.BillToEmail, emailTemplate.Subject(model), emailTemplate.Build(model),
                sourceReference: $"Invoice:{invoice.Id}:{kind}", cancellationToken: cancellationToken);
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
