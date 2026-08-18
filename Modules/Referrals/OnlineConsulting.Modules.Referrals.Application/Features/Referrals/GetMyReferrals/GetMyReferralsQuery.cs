using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetMyReferrals;

/// <summary>The referrals the caller has made as a referrer - who signed up with their code, and whether it's been rewarded yet.</summary>
public record GetMyReferralsQuery(Guid ReferrerUserId) : IRequest<OperationDataResult<List<ReferralResponse>>>;

public class GetMyReferralsHandler(IReferralRepository repository) : IRequestHandler<GetMyReferralsQuery, OperationDataResult<List<ReferralResponse>>>
{
    public async Task<OperationDataResult<List<ReferralResponse>>> Handle(GetMyReferralsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(r => r.ReferrerUserId == request.ReferrerUserId, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(ReferralResponse.FromDomain).ToList();

        return Result.Success(response, "Referrals retrieved successfully.");
    }
}
