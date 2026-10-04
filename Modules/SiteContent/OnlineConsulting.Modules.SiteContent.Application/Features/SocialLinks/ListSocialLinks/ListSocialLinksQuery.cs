using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.ListSocialLinks;

public record ListSocialLinksQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<SocialLinkResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(SocialLink.Name)]);
}

public class ListSocialLinksHandler(ISocialLinkRepository repository)
    : IRequestHandler<ListSocialLinksQuery, OperationDataResult<Paginate<SocialLinkResponse>>>
{
    public async Task<OperationDataResult<Paginate<SocialLinkResponse>>> Handle(ListSocialLinksQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

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
