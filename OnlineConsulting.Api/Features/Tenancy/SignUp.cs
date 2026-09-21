using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Bundles ReserveTenant/CreateTenantAdmin/ActivateTenantSubscription into one wire contract; the Api layer chains them as three ISender.Send calls, same pattern as SubscribeToMembership.cs.</summary>
public record SignUpTenantRequest(string CompanyName, string AdminFirstName, string AdminLastName, string AdminEmail, string AdminPassword, List<string> ModuleKeys, string PaymentMethodId, string? AdminPhoneNumber = null);

/// <summary>Pay-first: charges the card before creating any user, so a decline never leaves an orphan account; a rare post-charge user-creation failure triggers RollbackTenantSignupCommand to refund it.</summary>
public class SignUp : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/signup", Handle)
            .WithTags("Tenancy")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("SignUpTenant")
            .WithDescription("Charges the selected modules and, once payment succeeds, creates the tenant's first (admin) user.");
    }

    private static async Task<IResult> Handle([FromBody] SignUpTenantRequest request, ISender sender, HttpContext httpContext)
    {
        var reserveResult = await sender.Send(new ReserveTenantCommand(request.CompanyName, request.ModuleKeys, request.AdminEmail));

        if (!reserveResult.IsSuccessful || reserveResult.Data is null)
        {
            return reserveResult.ToEnvelopedResult(httpContext);
        }

        var tenantId = reserveResult.Data.TenantId;

        var activateResult = await sender.Send(new ActivateTenantSubscriptionCommand(tenantId, request.PaymentMethodId));

        if (!activateResult.IsSuccessful)
        {
            return activateResult.ToEnvelopedResult(httpContext);
        }

        var adminResult = await sender.Send(new CreateTenantAdminCommand(tenantId, request.AdminFirstName, request.AdminLastName, request.AdminEmail, request.AdminPassword, request.AdminPhoneNumber));

        if (!adminResult.IsSuccessful || adminResult.Data is null)
        {
            _ = await sender.Send(new RollbackTenantSignupCommand(tenantId));
            return adminResult.ToEnvelopedResult(httpContext);
        }

        var ownerResult = await sender.Send(new SetTenantOwnerCommand(tenantId, adminResult.Data.UserId));

        return ownerResult.ToEnvelopedResult(httpContext);
    }
}
