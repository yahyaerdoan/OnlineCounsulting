using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Notifications.Persistence;
using OnlineConsulting.Notifications.Sending;
using OnlineConsulting.SharedKernel.Notifications;
using Polly;
using Polly.Retry;

namespace OnlineConsulting.Notifications.Dispatch;

// Two retry layers, deliberately different scales:
//  - _sendPipeline (Polly, seconds): absorbs a single transient blip in the same dispatch pass
//    (a dropped connection, a momentary DNS hiccup) without burning an outbox Attempts slot.
//  - Attempts/NextAttemptAt on the row itself (minutes-to-an-hour, exponential, capped): survives
//    an outage that outlives one dispatch pass entirely - the row is durable in the DB regardless,
//    so the process can restart, the network can come back an hour later, nothing is lost until
//    MaxAttempts is exhausted and the row is marked Failed for manual/alerted follow-up.
public class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxDispatcherOptions> options,
    ILogger<OutboxDispatcher> logger) : BackgroundService
{
    private readonly ResiliencePipeline _sendPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 2,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(2),
        })
        .Build();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchDueBatchAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox dispatch cycle failed unexpectedly.");
            }

            await Task.Delay(settings.PollInterval, stoppingToken);
        }
    }

    private async Task DispatchDueBatchAsync(OutboxDispatcherOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var now = DateTimeOffset.UtcNow;
        var due = await context.OutboxEmails
            .Where(e => e.Status == OutboxEmailStatus.Pending && e.NextAttemptAt <= now)
            .OrderBy(e => e.NextAttemptAt)
            .Take(settings.BatchSize)
            .ToListAsync(cancellationToken);

        if (due.Count == 0)
            return;

        foreach (var email in due)
            await DispatchOneAsync(email, emailSender, settings, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchOneAsync(OutboxEmail email, IEmailSender emailSender, OutboxDispatcherOptions settings, CancellationToken cancellationToken)
    {
        email.Attempts++;

        try
        {
            await _sendPipeline.ExecuteAsync(ct => new ValueTask(emailSender.SendAsync(email.To, email.Subject, email.HtmlBody, email.Cc, ct)), cancellationToken);

            email.Status = OutboxEmailStatus.Sent;
            email.SentAt = DateTimeOffset.UtcNow;
            email.LastError = null;
            logger.LogInformation("Sent outbox email {EmailId} to {To} after {Attempts} attempt(s).", email.Id, email.To, email.Attempts);
        }
        catch (Exception ex)
        {
            email.LastError = ex.Message;

            if (email.Attempts >= settings.MaxAttempts)
            {
                email.Status = OutboxEmailStatus.Failed;
                logger.LogError(ex, "Outbox email {EmailId} to {To} permanently failed after {Attempts} attempts.", email.Id, email.To, email.Attempts);
            }
            else
            {
                var delay = TimeSpan.FromMinutes(Math.Pow(2, email.Attempts));
                email.NextAttemptAt = DateTimeOffset.UtcNow.Add(delay > settings.BackoffCap ? settings.BackoffCap : delay);
                logger.LogWarning(ex, "Outbox email {EmailId} to {To} failed (attempt {Attempts}/{MaxAttempts}), retrying at {NextAttemptAt}.",
                    email.Id, email.To, email.Attempts, settings.MaxAttempts, email.NextAttemptAt);
            }
        }
    }
}
