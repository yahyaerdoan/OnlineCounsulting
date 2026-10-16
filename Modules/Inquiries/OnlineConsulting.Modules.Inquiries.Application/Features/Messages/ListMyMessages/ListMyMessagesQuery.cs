using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ListMyMessages;

public record ListMyMessagesQuery(Guid UserId) : IRequest<OperationDataResult<List<MessageResponse>>>;

public class ListMyMessagesHandler(IMessageRepository repository) : IRequestHandler<ListMyMessagesQuery, OperationDataResult<List<MessageResponse>>>
{
    public async Task<OperationDataResult<List<MessageResponse>>> Handle(ListMyMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await repository.GetAllAsync(m => m.UserId == request.UserId, orderBy: q => q.OrderByDescending(m => m.LastMessageAt), cancellationToken: cancellationToken);
        return Result.Success(messages.Select(MessageResponse.FromDomain).ToList(), "Messages retrieved successfully.");
    }
}
