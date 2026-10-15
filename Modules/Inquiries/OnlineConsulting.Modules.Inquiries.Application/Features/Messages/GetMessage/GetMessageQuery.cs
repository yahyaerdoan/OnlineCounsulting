using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMessage;

public record GetMessageQuery(Guid Id) : IRequest<OperationDataResult<MessageResponse>>, ISecureAddRequest
{
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Read];
}

public class GetMessageHandler(IMessageRepository repository) : IRequestHandler<GetMessageQuery, OperationDataResult<MessageResponse>>
{
    public async Task<OperationDataResult<MessageResponse>> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await repository.GetWithRepliesAsync(m => m.Id == request.Id, cancellationToken);
        return message is null
            ? Result.NotFound<MessageResponse>(string.Format(MessageMessages.MessageNotFoundFormat, request.Id))
            : Result.Success(MessageResponse.FromDomain(message), "Message retrieved successfully.");
    }
}
