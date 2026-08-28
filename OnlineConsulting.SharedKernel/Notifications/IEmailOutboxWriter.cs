namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>TModule pins each module to its own writer at compile time.</summary>
public interface IEmailOutboxWriter<TModule>
{
    void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null);
}
