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

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.GetAllPartnershipsPaged;

public record GetAllPartnershipsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<PartnershipResponse>>>;

public class GetAllPartnershipsPagedHandler(IPartnershipRepository partnershipRepository, IPartnershipSocialLinkRepository socialLinkRepository, IFeatureFlagReader featureFlagReader)
    : IRequestHandler<GetAllPartnershipsPagedQuery, OperationDataResult<Paginate<PartnershipResponse>>>
{
    private const string PartnershipsFeatureFlagKey = "Partnerships";

    public async Task<OperationDataResult<Paginate<PartnershipResponse>>> Handle(GetAllPartnershipsPagedQuery request, CancellationToken cancellationToken)
    {
        if (!await featureFlagReader.IsEnabledAsync(PartnershipsFeatureFlagKey, cancellationToken))
        {
            return Result.Success(new Paginate<PartnershipResponse> { Items = [], Index = request.PageRequest.PageIndex, Size = request.PageRequest.PageSize, Count = 0, Pages = 0 }, "Partnerships is disabled for this tenant.");
        }

        var paged = await partnershipRepository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var partnershipIds = paged.Items.Select(x => x.Id).ToHashSet();
        var socialLinks = await socialLinkRepository.GetListAsync(x => partnershipIds.Contains(x.PartnershipId), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var socialLinksByPartnershipId = socialLinks.Items.ToLookup(x => x.PartnershipId);

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
