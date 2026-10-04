using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.ListPartnerships;

/// <summary>Paged counterpart to GetAllPartnershipsQuery - same "Partnerships" feature-flag gate, returns an empty page instead of the full list when disabled.</summary>
public record ListPartnershipsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<PartnershipResponse>>>;

public class ListPartnershipsHandler(IPartnershipRepository partnershipRepository, IPartnershipSocialLinkRepository socialLinkRepository, IFeatureFlagReader featureFlagReader)
    : IRequestHandler<ListPartnershipsQuery, OperationDataResult<Paginate<PartnershipResponse>>>
{
    private const string PartnershipsFeatureFlagKey = "Partnerships";

    public async Task<OperationDataResult<Paginate<PartnershipResponse>>> Handle(ListPartnershipsQuery request, CancellationToken cancellationToken)
    {
        if (!await featureFlagReader.IsEnabledAsync(PartnershipsFeatureFlagKey, cancellationToken))
        {
            return Result.Success(new Paginate<PartnershipResponse> { Items = [], Index = request.PageRequest.PageIndex, Size = request.PageRequest.PageSize, Count = 0, Pages = 0 }, "Partnerships is disabled for this tenant.");
        }

        var paged = await partnershipRepository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var partnershipIds = paged.Items.Select(x => x.Id).ToHashSet();
        var socialLinks = await socialLinkRepository.GetAllAsync(x => partnershipIds.Contains(x.PartnershipId), cancellationToken: cancellationToken);
        var socialLinksByPartnershipId = socialLinks.ToLookup(x => x.PartnershipId);

        var response = new Paginate<PartnershipResponse>
        {
            Items = [.. paged.Items.Select(p => PartnershipResponse.FromDomain(p, [.. socialLinksByPartnershipId[p.Id].Select(PartnershipSocialLinkResponse.FromDomain)]))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Partnerships retrieved successfully.");
    }
}
