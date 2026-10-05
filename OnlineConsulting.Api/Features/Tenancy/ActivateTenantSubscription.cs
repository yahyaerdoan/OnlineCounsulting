using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RetryTenantSubscriptionActivation;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Retries billing for a tenant SignUp.cs already created but couldn't activate (e.g. a Stripe error); the admin logs in and retries here since /signup can't be resubmitted once the user exists.</summary>
public class ActivateTenantSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/{tenantId:guid}/activate", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ActivateTenantSubscription")
            .WithDescription("Retries billing for a tenant that was reserved and given an admin user but whose subscription activation previously failed.")
            .ProducesEnveloped<ActivateTenantSubscriptionResult>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(Guid tenantId, [FromBody] ActivateTenantSubscriptionRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(tenantId))).ToEnvelopedResult(httpContext);
}

public record ActivateTenantSubscriptionRequest(string PaymentMethodId)
{
    public RetryTenantSubscriptionActivationCommand ToCommand(Guid tenantId) => new(tenantId, PaymentMethodId);
}
