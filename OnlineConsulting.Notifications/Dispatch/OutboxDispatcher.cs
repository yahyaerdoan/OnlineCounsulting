using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Notifications.Persistence;
using OnlineConsulting.Notifications.Sending;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using System.Net;
using Polly;
using Polly.Retry;

namespace OnlineConsulting.Notifications.Dispatch;

/// <summary>Dispatches due outbox emails using two retry layers: fast in-process Polly retries and slower durable Attempts/NextAttemptAt backoff on the row itself.</summary>
public class OutboxDispatcher(IServiceScopeFactory scopeFactory, IOptions<OutboxDispatcherOptions> options, ILogger<OutboxDispatcher> logger) : BackgroundService
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
        using var timer = new PeriodicTimer(settings.PollInterval);

        do
        {
            try
            {
                await DispatchDueBatchAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox dispatch cycle failed unexpectedly.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    /// <summary>Sends run in parallel, but entity mutations are applied afterward on this thread since ChangeTracker isn't thread-safe.</summary>
    private async Task DispatchDueBatchAsync(OutboxDispatcherOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var now = DateTimeOffset.UtcNow;
        var due = await context.OutboxEmails.Where(e => e.Status == OutboxEmailStatus.Pending && e.NextAttemptAt <= now).OrderBy(e => e.NextAttemptAt).Take(settings.BatchSize).ToListAsync(cancellationToken);

        if (due.Count == 0)
        {
            return;
        }

        var brandReader = scope.ServiceProvider.GetRequiredService<ITenantBrandReader>();
        var mediaUrlReader = scope.ServiceProvider.GetRequiredService<IMediaAssetUrlReader>();
        var brands = new Dictionary<Guid, (string Name, string Header)>();
        foreach (var tenantId in due.Select(e => e.TenantId).Distinct())
        {
            var brand = await brandReader.GetAsync(tenantId, cancellationToken);
            var logoUrl = brand.LogoMediaAssetId is Guid logoId ? await mediaUrlReader.GetPublicUrlAsync(logoId, cancellationToken) : null;
            brands[tenantId] = (brand.Name, LogoHeader(logoUrl, brand.Name));
        }

        var outcomes = new Exception?[due.Count];

        await Parallel.ForEachAsync(Enumerable.Range(0, due.Count),
            new ParallelOptions { MaxDegreeOfParallelism = settings.MaxConcurrentSends, CancellationToken = cancellationToken },
            async (i, ct) => outcomes[i] = await SendAsync(due[i], brands[due[i].TenantId], emailSender, ct));

        for (var i = 0; i < due.Count; i++)
        {
            Apply(due[i], outcomes[i], settings);
        }

        _ = await context.SaveChangesAsync(cancellationToken);
    }

    private static string LogoHeader(string? logoUrl, string brandName) => logoUrl is null
        ? string.Empty
        : $"""<p style="margin: 0 0 16px;"><img src="{WebUtility.HtmlEncode(logoUrl)}" alt="{WebUtility.HtmlEncode(brandName)}" style="max-height: 48px; max-width: 200px;" /></p>""";

    private async Task<Exception?> SendAsync(OutboxEmail email, (string Name, string Header) brand, IEmailSender emailSender, CancellationToken cancellationToken)
    {
        var subject = email.Subject.Replace(EmailLayout.BrandToken, brand.Name, StringComparison.Ordinal);
        var htmlBody = email.HtmlBody
            .Replace(EmailLayout.BrandHeaderToken, brand.Header, StringComparison.Ordinal)
            .Replace(EmailLayout.BrandToken, WebUtility.HtmlEncode(brand.Name), StringComparison.Ordinal);

        try
        {
            await _sendPipeline.ExecuteAsync(ct => new ValueTask(emailSender.SendAsync(email.To, subject, htmlBody, email.Cc, brand.Name, ct)), cancellationToken);

            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private void Apply(OutboxEmail email, Exception? sendError, OutboxDispatcherOptions settings)
    {
        email.Attempts++;

        if (sendError is null)
        {
            email.Status = OutboxEmailStatus.Sent;
            email.SentAt = DateTimeOffset.UtcNow;
            email.LastError = null;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Sent outbox email {EmailId} to {To} after {Attempts} attempt(s).", email.Id, email.To, email.Attempts);
            }

            return;
        }

        email.LastError = sendError.Message;

        if (email.Attempts >= settings.MaxAttempts)
        {
            email.Status = OutboxEmailStatus.Failed;

            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(sendError, "Outbox email {EmailId} to {To} permanently failed after {Attempts} attempts.", email.Id, email.To, email.Attempts);
            }
        }
        else
        {
            var delay = TimeSpan.FromMinutes(Math.Pow(2, email.Attempts));

            email.NextAttemptAt = DateTimeOffset.UtcNow.Add(delay > settings.BackoffCap ? settings.BackoffCap : delay);

            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(sendError, "Outbox email {EmailId} to {To} failed (attempt {Attempts}/{MaxAttempts}), retrying at {NextAttemptAt}.",
                    email.Id, email.To, email.Attempts, settings.MaxAttempts, email.NextAttemptAt);
            }
        }
    }
}
