using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ListMessages;

public record ListMessagesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<MessageResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Read];
}

public class ListMessagesHandler(IMessageRepository repository)
    : IRequestHandler<ListMessagesQuery, OperationDataResult<Paginate<MessageResponse>>>
{
    public async Task<OperationDataResult<Paginate<MessageResponse>>> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.CreatedDate, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<MessageResponse>
        {
            Items = [.. paged.Items.Select(MessageResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Messages retrieved successfully.");
    }
}
