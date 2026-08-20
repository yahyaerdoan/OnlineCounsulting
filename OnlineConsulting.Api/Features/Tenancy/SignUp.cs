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

/// <summary>Public self-service tenant signup. Order is deliberate and closes a previously-documented residual race: (1) ReserveTenantCommand (Tenancy) creates the Tenant/TenantSubscription/Pending TenantSubscriptionItem rows - cheap, local, no billing; (2) CreateTenantAdminCommand (Identity) creates the first user - UserManager.CreateAsync's own unique index on email is the real, atomic, DB-enforced guard against duplicate signups, and it now runs BEFORE any billing, so a rejected duplicate email never leaves an orphaned Stripe charge behind; (3) SetTenantOwnerCommand (Tenancy) records that new user as Tenant.OwnerUserId, distinguishing the actual owner from any other Admin-role user later invited into the same tenant; only then (4) ActivateTenantSubscriptionCommand (Tenancy) actually charges the card. No UserManager.FindByEmailAsync pre-check here anymore - it used to run first specifically to protect against paying for a tenant that could end up with no user, but that's no longer possible now that user creation happens before billing, so the pre-check was redundant (and never fully closed the race itself - see previous ARCHITECTURE_MIGRATION.md entry). If step (4) fails after (1)-(3) succeeded, the Tenant/User already exist - a legitimate partial-success state, not a bug: the tenant is PendingPayment/Failed with a real admin user who can log in and retry billing via POST /api/tenancy/{tenantId}/activate (ActivateTenantSubscription.cs) instead of registering again.</summary>
public class SignUp : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/signup", Handle)
            .WithTags("Tenancy")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("SignUpTenant")
            .WithDescription("Creates a new tenant, its first (admin) user, and starts its subscription for the selected modules.");
    }

    private static async Task<IResult> Handle([FromBody] SignUpTenantRequest request, ISender sender, HttpContext httpContext)
    {
        var reserveResult = await sender.Send(new ReserveTenantCommand(request.CompanyName, request.ModuleKeys, request.AdminEmail));
        if (!reserveResult.IsSuccessful || reserveResult.Data is null)
        {
            return reserveResult.ToEnvelopedResult(httpContext);
        }

        var tenantId = reserveResult.Data.TenantId;

        var adminResult = await sender.Send(new CreateTenantAdminCommand(
            tenantId, request.AdminFirstName, request.AdminLastName, request.AdminEmail, request.AdminPassword, request.AdminPhoneNumber));
        if (!adminResult.IsSuccessful || adminResult.Data is null)
        {
            return adminResult.ToEnvelopedResult(httpContext);
        }

        var ownerResult = await sender.Send(new SetTenantOwnerCommand(tenantId, adminResult.Data.UserId));
        if (!ownerResult.IsSuccessful)
        {
            return ownerResult.ToEnvelopedResult(httpContext);
        }

        var activateResult = await sender.Send(new ActivateTenantSubscriptionCommand(tenantId, request.PaymentMethodId));
        return activateResult.ToEnvelopedResult(httpContext);
    }
}
