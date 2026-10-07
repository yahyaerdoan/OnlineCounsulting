namespace OnlineConsulting.SharedKernel.Notifications.Templates;

/// <summary>Wraps an email body in the shared font/spacing/footer used by every template.</summary>
public static class EmailLayout
{
    /// <summary>Stands for the sending business's name in a subject or body; OutboxDispatcher fills it in per tenant when the email goes out.</summary>
    public const string BrandToken = "{{brand}}";

    /// <summary>Top of every email; OutboxDispatcher replaces it with the business's logo, or nothing when it has none.</summary>
    public const string BrandHeaderToken = "{{brand-header}}";

    public static string Wrap(string bodyHtml) => $"""
        <div style="font-family: Arial, sans-serif; font-size: 14px; color: #333333; line-height: 1.5;">
            {BrandHeaderToken}
            {bodyHtml}
            <hr style="border: none; border-top: 1px solid #eeeeee; margin: 24px 0;" />
            <p style="font-size: 12px; color: #999999;">{BrandToken}</p>
        </div>
        """;
}
