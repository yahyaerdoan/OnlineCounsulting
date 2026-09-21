using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Retries billing for a tenant SignUp.cs already created but couldn't activate (e.g. a Stripe error); the admin logs in and retries here since /signup can't be resubmitted once the user exists. Ownership is checked here, not in ActivateTenantSubscriptionCommand, because that command also runs anonymously from SignUp.cs's own orchestration with no JWT/tenant claim available.</summary>
public record ActivateTenantSubscriptionRequest(string PaymentMethodId);

public class ActivateTenantSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/{tenantId:guid}/activate", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ActivateTenantSubscription")
            .WithDescription("Retries billing for a tenant that was reserved and given an admin user but whose subscription activation previously failed.");
    }

    private static async Task<IResult> Handle(Guid tenantId, [FromBody] ActivateTenantSubscriptionRequest request, ISender sender, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor, HttpContext httpContext)
    {
        if (!TenantOwnershipGuard.CallerMayManage(tenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden<ActivateTenantSubscriptionResult>(TenantSubscriptionItemMessages.NotAuthorizedForTenant).ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ActivateTenantSubscriptionCommand(tenantId, request.PaymentMethodId));

        return result.ToEnvelopedResult(httpContext);
    }
}
