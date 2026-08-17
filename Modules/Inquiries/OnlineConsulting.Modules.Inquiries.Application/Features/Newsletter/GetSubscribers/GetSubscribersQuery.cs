using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetSubscribers;

public record GetSubscribersQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<NewsletterSubscriberResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [NewsletterOperationClaims.Admin, NewsletterOperationClaims.Read];
}

public class GetSubscribersHandler(INewsletterSubscriberRepository repository)
    : IRequestHandler<GetSubscribersQuery, OperationDataResult<Paginate<NewsletterSubscriberResponse>>>
{
    public async Task<OperationDataResult<Paginate<NewsletterSubscriberResponse>>> Handle(GetSubscribersQuery request, CancellationToken cancellationToken)
    {
        var subscribers = await repository.GetListAsync(orderBy: q => q.OrderByDescending(s => s.CreatedDate), index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<NewsletterSubscriberResponse>
        {
            Items = [.. subscribers.Items.Select(NewsletterSubscriberResponse.FromDomain)],
            Index = subscribers.Index,
            Size = subscribers.Size,
            Count = subscribers.Count,
            Pages = subscribers.Pages,
        };

        return Result.Success(response, "Subscribers retrieved successfully.");
    }
}
