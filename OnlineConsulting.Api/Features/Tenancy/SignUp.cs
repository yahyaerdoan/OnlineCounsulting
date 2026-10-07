using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.ValidateTenantAdmin;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.ActivateTenantSubscription;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.ReserveTenant;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RollbackTenantSignup;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.SetTenantOwner;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.SendTenantSignupReceipt;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Facade;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Bundles ReserveTenant/CreateTenantAdmin/ActivateTenantSubscription into one wire contract; the Api layer chains them as three ISender.Send calls, same pattern as SubscribeToMembership.cs.</summary>
public record SignUpTenantRequest(string CompanyName, string AdminFirstName, string AdminLastName, string AdminEmail, string AdminPassword, List<string> ModuleKeys, string PaymentMethodId, string? AdminPhoneNumber = null);

/// <summary>Pay-first: charges the card before creating any user, so a decline never leaves an orphan account. The admin account's fields are
/// validated before anything is charged; a rare post-charge user-creation failure (e.g. a race) triggers RollbackTenantSignupCommand, which refunds it.</summary>
public class SignUp : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/signup", Handle)
            .WithTags("Tenancy")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("SignUpTenant")
            .WithDescription("Charges the selected modules and, once payment succeeds, creates the tenant's first (admin) user. Returns the tenant's own site, where the admin signs in.")
            .ProducesEnveloped<TenantSignupResult>();
    }

    private static async Task<IResult> Handle([FromBody] SignUpTenantRequest request, ISender sender, ITenantOriginReader originReader, HttpContext httpContext)
    {
        var accountCheck = await sender.Send(new ValidateTenantAdminQuery(request.AdminFirstName, request.AdminLastName, request.AdminEmail, request.AdminPassword));

        if (!accountCheck.IsSuccessful)
        {
            return accountCheck.ToEnvelopedResult(httpContext);
        }

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

        if (!ownerResult.IsSuccessful)
        {
            return ownerResult.ToEnvelopedResult(httpContext);
        }

        _ = await sender.Send(new SendTenantSignupReceiptCommand(tenantId));

        var siteUrl = await originReader.GetOriginAsync(tenantId, httpContext.RequestAborted);
        return Result.Success(new TenantSignupResult(tenantId, siteUrl), ownerResult.Title).ToEnvelopedResult(httpContext);
    }
}
