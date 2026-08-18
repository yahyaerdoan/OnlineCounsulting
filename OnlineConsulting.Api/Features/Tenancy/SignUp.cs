using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Public self-service tenant signup. Two separate MediatR sends, not one handler crossing module boundaries: SignUpTenantCommand (Tenancy) creates the Tenant/TenantSubscription/TenantSubscriptionItem rows and starts billing, then CreateTenantAdminCommand (Identity) creates the first user for that tenant - same chaining pattern SubscribeToMembership.cs already uses across Memberships/Referrals. If the second send fails, the tenant/subscription already created by the first send are not rolled back (each command is its own transaction via ITransactionAddRequest) - a known gap, flagged in ARCHITECTURE_MIGRATION.md, until an invite/retry flow exists.</summary>
public class SignUp : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tenancy/signup", Handle)
            .WithTags("Tenancy")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("SignUpTenant")
            .WithDescription("Creates a new tenant, starts its subscription for the selected modules, and creates its first (admin) user.");
    }

    private static async Task<IResult> Handle([FromBody] SignUpTenantCommand command, ISender sender, HttpContext httpContext)
    {
        var signupResult = await sender.Send(command);
        if (!signupResult.IsSuccessful || signupResult.Data is null)
            return signupResult.ToEnvelopedResult(httpContext);

        var adminResult = await sender.Send(new CreateTenantAdminCommand(
            signupResult.Data.TenantId, command.AdminFirstName, command.AdminLastName, command.AdminEmail, command.AdminPassword));

        if (!adminResult.IsSuccessful)
            return adminResult.ToEnvelopedResult(httpContext);

        return signupResult.ToEnvelopedResult(httpContext);
    }
}
