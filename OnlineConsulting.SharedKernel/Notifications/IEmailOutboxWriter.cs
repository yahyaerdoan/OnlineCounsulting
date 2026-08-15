namespace OnlineConsulting.SharedKernel.Notifications;

public interface IEmailOutboxWriter
{
    void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null);
}
