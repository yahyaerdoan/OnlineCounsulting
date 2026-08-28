using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Public self-service tenant signup wire contract - bundles what used to be split across ReserveTenantCommand/CreateTenantAdminCommand/ActivateTenantSubscriptionCommand into the one request shape the client actually submits. Not itself an IRequest: this orchestration lives in the Api layer as three separate ISender.Send calls, not one handler crossing module boundaries, same chaining pattern SubscribeToMembership.cs already uses across Memberships/Referrals.</summary>
public record SignUpTenantRequest(
    string CompanyName,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    List<string> ModuleKeys,
    string PaymentMethodId,
    string? AdminPhoneNumber = null);

/// <summary>Public self-service tenant signup - pay-first. Charges the card (ActivateTenantSubscriptionCommand) before creating any user/email, so a decline never leaves an Identity account behind. If user creation fails afterward (rare concurrent-duplicate-email race), RollbackTenantSignupCommand cancels the just-captured charge - nobody pays for a tenant with no admin user.</summary>
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

        var adminResult = await sender.Send(new CreateTenantAdminCommand(
            tenantId, request.AdminFirstName, request.AdminLastName, request.AdminEmail, request.AdminPassword, request.AdminPhoneNumber));
        if (!adminResult.IsSuccessful || adminResult.Data is null)
        {
            _ = await sender.Send(new RollbackTenantSignupCommand(tenantId));
            return adminResult.ToEnvelopedResult(httpContext);
        }

        var ownerResult = await sender.Send(new SetTenantOwnerCommand(tenantId, adminResult.Data.UserId));
        return ownerResult.ToEnvelopedResult(httpContext);
    }
}
