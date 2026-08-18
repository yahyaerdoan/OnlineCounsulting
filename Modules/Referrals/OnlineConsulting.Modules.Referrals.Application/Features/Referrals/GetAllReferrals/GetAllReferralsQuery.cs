using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetAllReferrals;

public record GetAllReferralsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<ReferralResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ReferralsOperationClaims.Admin, ReferralsOperationClaims.Read];
}

public class GetAllReferralsHandler(IReferralRepository repository) : IRequestHandler<GetAllReferralsQuery, OperationDataResult<Paginate<ReferralResponse>>>
{
    public async Task<OperationDataResult<Paginate<ReferralResponse>>> Handle(GetAllReferralsQuery request, CancellationToken cancellationToken)
    {
        var referrals = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<ReferralResponse>
        {
            Items = [.. referrals.Items.Select(ReferralResponse.FromDomain)],
            Index = referrals.Index,
            Size = referrals.Size,
            Count = referrals.Count,
            Pages = referrals.Pages,
        };

        return Result.Success(response, "Referrals retrieved successfully.");
    }
}
