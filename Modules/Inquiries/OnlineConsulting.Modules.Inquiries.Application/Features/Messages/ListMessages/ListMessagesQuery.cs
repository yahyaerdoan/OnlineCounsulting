using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ListMessages;

public record ListMessagesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<MessageResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Message.FirstName), nameof(Message.LastName), nameof(Message.Email), nameof(Message.Subject), nameof(Message.CreatedDate)]);

    [JsonIgnore]
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Read];
}

public class ListMessagesHandler(IMessageRepository repository)
    : IRequestHandler<ListMessagesQuery, OperationDataResult<Paginate<MessageResponse>>>
{
    public async Task<OperationDataResult<Paginate<MessageResponse>>> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.CreatedDate, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

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
