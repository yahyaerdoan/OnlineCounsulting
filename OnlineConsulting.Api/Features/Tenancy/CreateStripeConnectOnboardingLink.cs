using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CreateStripeConnectOnboardingLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CreateStripeConnectOnboardingLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/stripe-connect/onboarding-link", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CreateStripeConnectOnboardingLink")
            .WithDescription("Creates (or reuses) the caller's own tenant's Stripe Connect account and returns a fresh onboarding link to redirect to. Tenant admins only.")
            .ProducesEnveloped<StripeConnectOnboardingLinkResponse>();
    }

    private static async Task<IResult> Handle([FromBody] CreateStripeConnectOnboardingLinkRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

/// <summary>ReturnUrl is reused as both Stripe's refresh_url and return_url - the onboarding return page re-checks status either way.</summary>
public record CreateStripeConnectOnboardingLinkRequest(string ReturnUrl)
{
    public CreateStripeConnectOnboardingLinkCommand ToCommand() => new(ReturnUrl);
}
