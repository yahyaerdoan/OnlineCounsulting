using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Api.Features.Payments;

/// <summary>One route for every provider - {provider} picks the keyed IPaymentGateway that knows how to verify that provider's own signature scheme. Reads the raw body directly instead of model-binding JSON, since signature verification (Stripe's HMAC in particular) is computed over the exact original bytes.</summary>
public class PaymentWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/webhooks/{provider}", Handle)
            .WithTags("Payments")
            .WithName("PaymentWebhook")
            .WithDescription("Receives async payment status callbacks from a provider (e.g. Stripe) and notifies the owning module (Commerce/Scheduling) once verified.");
    }

    private static async Task<IResult> Handle(string provider, HttpContext httpContext, IServiceProvider serviceProvider, IPublisher publisher, CancellationToken cancellationToken)
    {
        var gateway = serviceProvider.GetKeyedService<IPaymentGateway>(provider);
        if (gateway is null)
            return Results.NotFound($"Unknown payment provider '{provider}'.");

        using var reader = new StreamReader(httpContext.Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = httpContext.Request.Headers["Stripe-Signature"].FirstOrDefault();

        var webhookEvent = await gateway.VerifyAndParseWebhookAsync(rawBody, signatureHeader, cancellationToken);
        if (webhookEvent is null)
            return Results.Ok(); // Unrecognized/irrelevant event type - acknowledge so the provider stops retrying.

        if (webhookEvent.Succeeded)
            await publisher.Publish(new PaymentSucceededNotification(webhookEvent.ReferenceId, webhookEvent.ProviderPaymentId, gateway.ProviderName), cancellationToken);
        else
            await publisher.Publish(new PaymentFailedNotification(webhookEvent.ReferenceId, webhookEvent.ProviderPaymentId, gateway.ProviderName), cancellationToken);

        return Results.Ok();
    }
}
