using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantBranding;

/// <summary>The caller's tenant's brand. Bypasses the status check so even a suspended business's site can show its own name.</summary>
public record GetTenantBrandingQuery : IRequest<OperationDataResult<TenantBrandingResponse>>, IBypassesTenantStatusCheck;

public class GetTenantBrandingHandler(ITenantProvider tenantProvider, ITenantBrandReader brandReader, ITenantOriginReader originReader)
    : IRequestHandler<GetTenantBrandingQuery, OperationDataResult<TenantBrandingResponse>>
{
    public async Task<OperationDataResult<TenantBrandingResponse>> Handle(GetTenantBrandingQuery request, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.TenantId;
        var brand = await brandReader.GetAsync(tenantId, cancellationToken);
        var platformUrl = await originReader.GetOriginAsync(TenantDefaults.DefaultTenantId, cancellationToken);

        return Result.Success(new TenantBrandingResponse(brand.Name, brand.LogoMediaAssetId, tenantId == TenantDefaults.DefaultTenantId, platformUrl), "Branding retrieved successfully.");
    }
}
