using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetBusinessTimeZone;

/// <summary>The caller's tenant's time zone; anonymous callers get the default tenant's.</summary>
public record GetBusinessTimeZoneQuery : IRequest<OperationDataResult<BusinessTimeZoneResponse>>;

public class GetBusinessTimeZoneHandler(ITenantProvider tenantProvider, ITenantTimeZoneReader timeZoneReader)
    : IRequestHandler<GetBusinessTimeZoneQuery, OperationDataResult<BusinessTimeZoneResponse>>
{
    public async Task<OperationDataResult<BusinessTimeZoneResponse>> Handle(GetBusinessTimeZoneQuery request, CancellationToken cancellationToken)
    {
        var zone = await timeZoneReader.GetAsync(tenantProvider.TenantId, cancellationToken);
        return Result.Success(new BusinessTimeZoneResponse(zone.Id), "Business time zone retrieved successfully.");
    }
}
