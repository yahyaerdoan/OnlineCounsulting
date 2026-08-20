using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Retries billing alone for a tenant already reserved and admin-created by SignUp.cs but not yet Active (its ActivateTenantSubscriptionCommand step failed, e.g. a Stripe error) - the original /api/tenancy/signup can't be resubmitted at that point since AdminEmail/AdminPassword no longer make sense once the user already exists. Authenticated: the tenant's own admin logs in (their account was created regardless of billing outcome) and calls this. TenantOwnershipGuard check happens here rather than inside ActivateTenantSubscriptionCommand itself, because that command is also sent anonymously from SignUp.cs's own internal orchestration (no JWT/tenant claim exists at that point) and so cannot itself depend on ITenantProvider - see ActivateTenantSubscriptionCommand's doc comment.</summary>
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

    private static async Task<IResult> Handle(
        Guid tenantId, [FromBody] ActivateTenantSubscriptionRequest request, ISender sender,
        ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor, HttpContext httpContext)
    {
        if (!TenantOwnershipGuard.CallerMayManage(tenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden<ActivateTenantSubscriptionResult>(TenantSubscriptionItemMessages.NotAuthorizedForTenant).ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ActivateTenantSubscriptionCommand(tenantId, request.PaymentMethodId));
        return result.ToEnvelopedResult(httpContext);
    }
}
