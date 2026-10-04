using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.ListNewsletterSubscribers;

public record ListNewsletterSubscribersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<NewsletterSubscriberResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [NewsletterOperationClaims.Admin, NewsletterOperationClaims.Read];
}

public class ListNewsletterSubscribersHandler(INewsletterSubscriberRepository repository)
    : IRequestHandler<ListNewsletterSubscribersQuery, OperationDataResult<Paginate<NewsletterSubscriberResponse>>>
{
    public async Task<OperationDataResult<Paginate<NewsletterSubscriberResponse>>> Handle(ListNewsletterSubscribersQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.CreatedDate, tieBreaker: x => x.Id, cancellationToken);

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
