using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Api.Features.Payments;

/// <summary>Separate endpoint from PaymentWebhook/SubscriptionWebhook: Stripe's Connect webhook (account.updated etc.) is configured as its own
/// endpoint in the dashboard, with its own signing secret, since it carries every connected account's events rather than the platform's own.</summary>
public class ConnectAccountWebhook : IVersionNeutralEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/payments/webhooks/{provider}/connect-accounts", Handle)
            .WithTags("Payments")
            .WithName("ConnectAccountWebhook")
            .WithDescription("Receives Stripe Connect account-capability callbacks and applies them to the owning tenant. Unrecognized/irrelevant event types are acknowledged with 200 OK so the provider stops retrying.")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(string provider, HttpContext httpContext, IServiceProvider serviceProvider, StripeConnectStatusUpdater statusUpdater, CancellationToken cancellationToken)
    {
        var gateway = serviceProvider.GetKeyedService<IConnectAccountGateway>(provider);

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

        await statusUpdater.ApplyAsync(webhookEvent.AccountId, webhookEvent.ChargesEnabled, webhookEvent.PayoutsEnabled, cancellationToken);

        return Results.Ok();
    }
}
