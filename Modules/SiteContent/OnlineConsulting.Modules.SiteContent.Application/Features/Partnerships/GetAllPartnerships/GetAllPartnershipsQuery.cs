using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.SharedKernel.FeatureFlags;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.GetAllPartnerships;

/// <summary>Public, matches GetAllTestimonialsQuery; the "Partnerships" flag key is duplicated as a literal since modules don't reference each other's Application projects.</summary>
public record GetAllPartnershipsQuery : IRequest<OperationDataResult<List<PartnershipResponse>>>;

public class GetAllPartnershipsHandler(IPartnershipRepository partnershipRepository, IPartnershipSocialLinkRepository socialLinkRepository, IFeatureFlagReader featureFlagReader)
    : IRequestHandler<GetAllPartnershipsQuery, OperationDataResult<List<PartnershipResponse>>>
{
    private const string PartnershipsFeatureFlagKey = "Partnerships";

    public async Task<OperationDataResult<List<PartnershipResponse>>> Handle(GetAllPartnershipsQuery request, CancellationToken cancellationToken)
    {
        if (!await featureFlagReader.IsEnabledAsync(PartnershipsFeatureFlagKey, cancellationToken))
        {
            return Result.Success(new List<PartnershipResponse>(), "Partnerships is disabled for this tenant.");
        }

        var partnerships = await partnershipRepository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var socialLinks = await socialLinkRepository.GetAllAsync(cancellationToken: cancellationToken);
        var socialLinksByPartnershipId = socialLinks.ToLookup(x => x.PartnershipId);

        var response = partnerships
            .Select(p => PartnershipResponse.FromDomain(p, [.. socialLinksByPartnershipId[p.Id].Select(PartnershipSocialLinkResponse.FromDomain)]))
            .ToList();

        return Result.Success(response, "Partnerships retrieved successfully.");
    }
}
