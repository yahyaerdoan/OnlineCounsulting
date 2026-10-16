using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMyMessage;

public record GetMyMessageQuery(Guid UserId, Guid Id) : IRequest<OperationDataResult<MessageResponse>>;

public class GetMyMessageHandler(IMessageRepository repository) : IRequestHandler<GetMyMessageQuery, OperationDataResult<MessageResponse>>
{
    public async Task<OperationDataResult<MessageResponse>> Handle(GetMyMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await repository.GetWithRepliesAsync(m => m.Id == request.Id && m.UserId == request.UserId, cancellationToken);
        return message is null
            ? Result.NotFound<MessageResponse>(string.Format(MessageMessages.MessageNotFoundFormat, request.Id))
            : Result.Success(MessageResponse.FromDomain(message), "Message retrieved successfully.");
    }
}
