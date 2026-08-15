using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMessages;

public record GetMessagesQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<MessageResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Read];
}

public class GetMessagesHandler(IMessageRepository repository)
    : IRequestHandler<GetMessagesQuery, OperationDataResult<Paginate<MessageResponse>>>
{
    public async Task<OperationDataResult<Paginate<MessageResponse>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await repository.GetListAsync(
            orderBy: q => q.OrderByDescending(m => m.CreatedDate),
            index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<MessageResponse>
        {
            Items = [.. messages.Items.Select(MessageResponse.FromDomain)],
            Index = messages.Index,
            Size = messages.Size,
            Count = messages.Count,
            Pages = messages.Pages,
        };

        return Result.Success(response, "Messages retrieved successfully.");
    }
}
