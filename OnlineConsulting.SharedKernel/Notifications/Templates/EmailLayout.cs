namespace OnlineConsulting.SharedKernel.Notifications.Templates;

// Shared wrapper so every template gets the same fonts/spacing/footer without repeating inline
// styles per template (every module's templates call this, never inline their own <div>/<style>).
public static class EmailLayout
{
    public static string Wrap(string bodyHtml) => $"""
        <div style="font-family: Arial, sans-serif; font-size: 14px; color: #333333; line-height: 1.5;">
            {bodyHtml}
            <hr style="border: none; border-top: 1px solid #eeeeee; margin: 24px 0;" />
            <p style="font-size: 12px; color: #999999;">OnlineConsulting</p>
        </div>
        """;
}
