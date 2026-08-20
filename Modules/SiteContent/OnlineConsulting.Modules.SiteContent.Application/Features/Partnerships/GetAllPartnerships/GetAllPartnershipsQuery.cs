using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.GetAllPartnerships;

/// <summary>Public - no login required, matches GetAllTestimonialsQuery. Gated by the "Partnerships" feature flag (FeatureFlagKeys.Partnerships in Modules/FeatureFlags's Application layer is the source of truth for this key string - duplicated as a literal here since modules don't reference each other's Application projects, same convention as cross-module ids).</summary>
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

        var partnerships = await partnershipRepository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var socialLinks = await socialLinkRepository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var socialLinksByPartnershipId = socialLinks.Items.ToLookup(x => x.PartnershipId);

        var response = partnerships.Items
            .Select(p => PartnershipResponse.FromDomain(p, [.. socialLinksByPartnershipId[p.Id].Select(PartnershipSocialLinkResponse.FromDomain)]))
            .ToList();

        return Result.Success(response, "Partnerships retrieved successfully.");
    }
}
