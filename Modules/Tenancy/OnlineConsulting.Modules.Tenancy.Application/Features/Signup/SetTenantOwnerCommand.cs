using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Third step of self-service tenant signup - records which Identity User is this tenant's actual
/// owner (Tenant.OwnerUserId), right after CreateTenantAdminCommand creates that user and before
/// ActivateTenantSubscriptionCommand bills the card. Deliberately plain IRequest, not ISecureAddRequest, for
/// the same reason as ActivateTenantSubscriptionCommand: OnlineConsulting.Api/Features/Tenancy/SignUp.cs sends
/// this as an internal step of the anonymous public signup endpoint, before any JWT/tenant claim exists.</summary>
public record SetTenantOwnerCommand(Guid TenantId, Guid OwnerUserId) : IRequest<OperationResult>;

public class SetTenantOwnerHandler(ITenantRepository tenantRepository) : IRequestHandler<SetTenantOwnerCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetTenantOwnerCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound(SignupMessages.TenantNotFound);
        }

        tenant.OwnerUserId = request.OwnerUserId;
        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant owner recorded successfully.");
    }
}
