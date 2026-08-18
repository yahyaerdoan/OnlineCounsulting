using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Abstractions;
using ResultHandler.Functional;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Registered once, globally (see Program.cs), so it runs for every module's requests rather than just Tenancy's - the same reason ITenantStatusReader lives here instead of Modules.Tenancy.Application, which no other module's Application layer may reference. Placed alongside AuthorizationAddingBehavior since it is conceptually the same kind of gate: "is this caller even allowed to use the app right now."
/// No ISecureAddRequest-style opt-out is used for the signup/login bypass; instead it relies on TenantProvider.TenantId falling back to TenantDefaults.DefaultTenantId for anonymous requests (no JWT tenant_id claim yet), which step 1 below already treats as "skip" - verified by reading TenantProvider.TenantId's implementation.</summary>
public class TenantStatusCheckBehavior<TRequest, TResponse>(
    ITenantProvider tenantProvider,
    ITenantStatusReader tenantStatusReader,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IOperationResult, IResultFailureFactory<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.TenantId;
        if (tenantId == TenantDefaults.DefaultTenantId)
            return await next(cancellationToken);

        var roles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        if (roles.Contains(GlobalOperationClaims.SuperAdmin))
            return await next(cancellationToken);

        var isSuspended = await tenantStatusReader.IsSuspendedAsync(tenantId, cancellationToken);
        if (isSuspended)
            return ResultFailureFactory.Forbidden<TResponse>("Subscription inactive - please update your billing to restore access.");

        return await next(cancellationToken);
    }
}
