using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Commerce.Application.Common.Templates;

public record OrderPaymentFailedEmailModel(string OrderNumber);

/// <summary>Sent when the provider reports the payment itself failed (card declined, etc.) - order is cancelled, invites a retry.</summary>
public class OrderPaymentFailedTemplate : IEmailTemplate<OrderPaymentFailedEmailModel>
{
    public string Subject(OrderPaymentFailedEmailModel model) => $"Payment failed for order {model.OrderNumber}";

    public string Build(OrderPaymentFailedEmailModel model) => EmailLayout.Wrap($"""
        <p>We couldn't process payment for your order.</p>
        <p>Order number: <strong>{WebUtility.HtmlEncode(model.OrderNumber)}</strong></p>
        <p>The order has been cancelled and you have not been charged. Please try again with a different payment method.</p>
        """);
}
