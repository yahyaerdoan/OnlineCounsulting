using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.GetAllSocialLinksPaged;

public record GetAllSocialLinksPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<SocialLinkResponse>>>;

public class GetAllSocialLinksPagedHandler(ISocialLinkRepository repository)
    : IRequestHandler<GetAllSocialLinksPagedQuery, OperationDataResult<Paginate<SocialLinkResponse>>>
{
    public async Task<OperationDataResult<Paginate<SocialLinkResponse>>> Handle(GetAllSocialLinksPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<SocialLinkResponse>
        {
            Items = [.. paged.Items.Select(SocialLinkResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Social links retrieved successfully.");
    }
}
