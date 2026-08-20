using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Api.Features.Payments;

/// <summary>One route for every subscription-capable provider - {provider} picks the keyed ISubscriptionGateway that knows how to verify that provider's own signature scheme. Separate from PaymentWebhook (one-time PaymentIntent events) since they're different interfaces/event vocabularies, even though today they're both served by Stripe.</summary>
public class SubscriptionWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/payments/webhooks/{provider}/subscriptions", Handle)
            .WithTags("Payments")
            .WithName("SubscriptionWebhook")
            .WithDescription("Receives async subscription-lifecycle callbacks (renewed/cancelled/payment failed) from a provider and notifies the owning module (Memberships) once verified.");
    }

    private static async Task<IResult> Handle(string provider, HttpContext httpContext, IServiceProvider serviceProvider, IPublisher publisher, CancellationToken cancellationToken)
    {
        var gateway = serviceProvider.GetKeyedService<ISubscriptionGateway>(provider);
        if (gateway is null)
        {
            return Results.NotFound($"Unknown payment provider '{provider}'.");
        }

        using var reader = new StreamReader(httpContext.Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = httpContext.Request.Headers["Stripe-Signature"].FirstOrDefault();

        var webhookEvent = await gateway.VerifyAndParseWebhookAsync(rawBody, signatureHeader, cancellationToken);
        if (webhookEvent is null)
        {
            return Results.Ok();
        }

        switch (webhookEvent.EventKind)
        {
            case SubscriptionEventKinds.Renewed:
                await publisher.Publish(new SubscriptionRenewedNotification(webhookEvent.ReferenceId, webhookEvent.ProviderSubscriptionId, webhookEvent.NewRenewalDate ?? DateTimeOffset.UtcNow), cancellationToken);
                break;
            case SubscriptionEventKinds.Cancelled:
                await publisher.Publish(new SubscriptionCancelledNotification(webhookEvent.ReferenceId, webhookEvent.ProviderSubscriptionId), cancellationToken);
                break;
            case SubscriptionEventKinds.PaymentFailed:
                await publisher.Publish(new SubscriptionPaymentFailedNotification(webhookEvent.ReferenceId, webhookEvent.ProviderSubscriptionId), cancellationToken);
                break;
        }

        return Results.Ok();
    }
}
