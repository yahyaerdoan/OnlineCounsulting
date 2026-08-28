namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>TModule pins each module to its own writer at compile time.</summary>
public interface IEmailOutboxWriter<TModule>
{
    /// <summary>Stages and flushes the outbox row via SaveChanges - rolls back with the caller if wrapped in ITransactionAddRequest.</summary>
    Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default);
}
