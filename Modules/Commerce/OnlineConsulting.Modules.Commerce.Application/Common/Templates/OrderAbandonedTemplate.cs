using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Commerce.Application.Common.Templates;

public record OrderAbandonedEmailModel(string OrderNumber);

/// <summary>Sent when checkout was started but never finished (tab closed, no payment attempt) and PendingOrderCleanupService
/// expired the order - softer tone than OrderPaymentFailedTemplate since nothing actually went wrong on our end.</summary>
public class OrderAbandonedTemplate : IEmailTemplate<OrderAbandonedEmailModel>
{
    public string Subject(OrderAbandonedEmailModel model) => $"Did you forget something? Order {model.OrderNumber} was not completed";

    public string Build(OrderAbandonedEmailModel model) => EmailLayout.Wrap($"""
        <p>It looks like you didn't finish checking out.</p>
        <p>Order number: <strong>{WebUtility.HtmlEncode(model.OrderNumber)}</strong></p>
        <p>We've cancelled it since no payment was received. If you'd still like to purchase, feel free to place a new order any time.</p>
        """);
}
