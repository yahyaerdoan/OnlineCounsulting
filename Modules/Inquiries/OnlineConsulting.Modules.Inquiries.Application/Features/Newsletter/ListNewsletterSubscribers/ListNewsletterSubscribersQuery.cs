using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.ListNewsletterSubscribers;

public record ListNewsletterSubscribersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<NewsletterSubscriberResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(NewsletterSubscriber.Email), nameof(NewsletterSubscriber.CreatedDate)]);

    public string[] Roles => [NewsletterOperationClaims.Admin, NewsletterOperationClaims.Read];
}

public class ListNewsletterSubscribersHandler(INewsletterSubscriberRepository repository)
    : IRequestHandler<ListNewsletterSubscribersQuery, OperationDataResult<Paginate<NewsletterSubscriberResponse>>>
{
    public async Task<OperationDataResult<Paginate<NewsletterSubscriberResponse>>> Handle(ListNewsletterSubscribersQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.CreatedDate, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var response = new Paginate<NewsletterSubscriberResponse>
        {
            Items = [.. paged.Items.Select(NewsletterSubscriberResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Newsletter subscribers retrieved successfully.");
    }
}
