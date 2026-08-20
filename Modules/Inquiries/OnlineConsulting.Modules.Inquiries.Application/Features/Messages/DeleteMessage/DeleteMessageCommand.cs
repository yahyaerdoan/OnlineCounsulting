using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.DeleteMessage;

public record DeleteMessageCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Delete];
}

public class DeleteMessageHandler(IMessageRepository repository) : IRequestHandler<DeleteMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await repository.GetAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
        if (message is null)
        {
            return Result.NotFound($"Message {request.Id} was not found.");
        }

        _ = await repository.DeleteAsync(message);

        return Result.Success("Message deleted successfully.");
    }
}
