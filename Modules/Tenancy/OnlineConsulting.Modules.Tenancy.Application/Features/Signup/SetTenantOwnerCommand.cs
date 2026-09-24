using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Third signup step - records the tenant's owner (Tenant.OwnerUserId) after CreateTenantAdminCommand; plain IRequest, not ISecureAddRequest, since SignUp.cs calls it internally before any JWT exists.</summary>
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
