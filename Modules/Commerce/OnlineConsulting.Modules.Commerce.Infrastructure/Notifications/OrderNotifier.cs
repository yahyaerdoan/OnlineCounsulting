using System.Globalization;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;

/// <summary>Emails go through the Commerce outbox (retried by the email dispatcher); notifications go through the inbox-recording sender,
/// so the bell shows them even when no device is registered. Each carries "orderId" so tapping it opens the order.</summary>
public class OrderNotifier(
    IEmailOutboxWriter<ICommerceOutboxModule> outboxWriter,
    IEmailTemplate<OrderConfirmationEmailModel> confirmationTemplate,
    IEmailTemplate<OrderPaymentFailedEmailModel> paymentFailedTemplate,
    IEmailTemplate<OrderAbandonedEmailModel> abandonedTemplate,
    IEmailTemplate<OrderRefundedEmailModel> refundedTemplate,
    IPushNotificationSender pushSender,
    IUserContactReader contactReader,
    IInvoiceService invoiceService,
    ILogger<OrderNotifier> logger) : IOrderNotifier
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public async Task PaidAsync(Order order, int itemCount, decimal total, Guid invoiceId, CancellationToken cancellationToken = default)
    {
        await EmailAsync(order, "Paid", confirmationTemplate, new OrderConfirmationEmailModel(order.OrderNumber, itemCount, total, invoiceService.ViewUrl(invoiceId)), cancellationToken);
        await PushAsync(order, "Payment received", $"Thanks! Order #{order.OrderNumber} is paid ({total.ToString("C", Usd)}). Your receipt is ready.", cancellationToken);
    }

    public async Task PaymentFailedAsync(Order order, CancellationToken cancellationToken = default)
    {
        await EmailAsync(order, "PaymentFailed", paymentFailedTemplate, new OrderPaymentFailedEmailModel(order.OrderNumber), cancellationToken);
        await PushAsync(order, "Payment didn't go through", $"Order #{order.OrderNumber} was cancelled because the payment failed. You can try again anytime.", cancellationToken);
    }

    public async Task AbandonedAsync(Order order, CancellationToken cancellationToken = default)
    {
        await EmailAsync(order, "Abandoned", abandonedTemplate, new OrderAbandonedEmailModel(order.OrderNumber), cancellationToken);
        await PushAsync(order, "Checkout not completed", $"Order #{order.OrderNumber} was cancelled because no payment came through. Your items are still waiting in the shop.", cancellationToken);
    }

    public async Task RefundedAsync(Order order, decimal? amount, CancellationToken cancellationToken = default)
    {
        await EmailAsync(order, "Refunded", refundedTemplate, new OrderRefundedEmailModel(order.OrderNumber, amount), cancellationToken);
        await PushAsync(order, "Your refund is on its way", $"Order #{order.OrderNumber} has been refunded. It can take a few business days to show on your statement.", cancellationToken);
    }

    private async Task EmailAsync<TModel>(Order order, string kind, IEmailTemplate<TModel> template, TModel model, CancellationToken cancellationToken)
    {
        try
        {
            if (await contactReader.GetEmailAsync(order.UserId, cancellationToken) is not { Length: > 0 } email)
            {
                logger.LogWarning("Order {OrderId}: no email on file for customer {UserId}, skipping the {Kind} email.", order.Id, order.UserId, kind);
                return;
            }

            await outboxWriter.EnqueueAsync(email, template.Subject(model), template.Build(model), sourceReference: $"Order:{order.Id}:{kind}", cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Order {OrderId}: queuing the {Kind} email failed.", order.Id, kind);
        }
    }

    private async Task PushAsync(Order order, string title, string body, CancellationToken cancellationToken)
    {
        try
        {
            await pushSender.SendToUserAsync(order.UserId, title, body, new Dictionary<string, string> { ["orderId"] = order.Id.ToString() }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Order {OrderId}: sending '{Title}' failed.", order.Id, title);
        }
    }
}
