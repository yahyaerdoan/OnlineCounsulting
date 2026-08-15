using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Commerce.Application.Common.Templates;

public record OrderConfirmationEmailModel(string OrderNumber, int ItemCount, decimal Total);

/// <summary>Order confirmation sent right after checkout.</summary>
public class OrderConfirmationTemplate : IEmailTemplate<OrderConfirmationEmailModel>
{
    public string Subject(OrderConfirmationEmailModel model) => $"Order confirmed: {model.OrderNumber}";

    public string Build(OrderConfirmationEmailModel model) => EmailLayout.Wrap($"""
        <p>Thanks for your order!</p>
        <p>Order number: <strong>{WebUtility.HtmlEncode(model.OrderNumber)}</strong></p>
        <p>{model.ItemCount} item(s), total: {model.Total:C}</p>
        <p>We'll notify you once your order ships.</p>
        """);
}
