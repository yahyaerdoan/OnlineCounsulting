using System.Globalization;
using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Commerce.Application.Common.Templates;

/// <summary>Amount is null for a full refund.</summary>
public record OrderRefundedEmailModel(string OrderNumber, decimal? Amount);

/// <summary>Sent when staff refund a paid order.</summary>
public class OrderRefundedTemplate : IEmailTemplate<OrderRefundedEmailModel>
{
    public string Subject(OrderRefundedEmailModel model) => $"Your refund for order {model.OrderNumber} is on its way";

    public string Build(OrderRefundedEmailModel model)
    {
        var what = model.Amount is { } amount
            ? $"A refund of <strong>{amount.ToString("C", CultureInfo.GetCultureInfo("en-US"))}</strong> has been issued"
            : "Your order has been refunded in full";

        return EmailLayout.Wrap($"""
            <p>{what}.</p>
            <p>Order number: <strong>{WebUtility.HtmlEncode(model.OrderNumber)}</strong></p>
            <p>It can take a few business days for the money to show on your statement, depending on your bank.</p>
            """);
    }
}
