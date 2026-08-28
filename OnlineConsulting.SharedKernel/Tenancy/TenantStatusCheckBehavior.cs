using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Abstractions;
using ResultHandler.Functional;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Global pipeline gate: is this caller even allowed to use the app right now. Anonymous requests (no tenant claim yet) bypass via TenantProvider.TenantId's Default fallback; IBypassesTenantStatusCheck opts specific commands out.</summary>
public class TenantStatusCheckBehavior<TRequest, TResponse>(ITenantProvider tenantProvider, ITenantStatusReader tenantStatusReader, IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IOperationResult, IResultFailureFactory<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IBypassesTenantStatusCheck)
        {
            return await next(cancellationToken);
        }

        var tenantId = tenantProvider.TenantId;
        if (tenantId == TenantDefaults.DefaultTenantId)
        {
            return await next(cancellationToken);
        }

        var roles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        if (roles.Contains(GlobalOperationClaims.SuperAdmin))
        {
            return await next(cancellationToken);
        }

        var isBlocked = await tenantStatusReader.IsBlockedAsync(tenantId, cancellationToken);
        return isBlocked
            ? ResultFailureFactory.Forbidden<TResponse>("Subscription inactive - please complete payment to access your account.")
            : await next(cancellationToken);
    }
}
