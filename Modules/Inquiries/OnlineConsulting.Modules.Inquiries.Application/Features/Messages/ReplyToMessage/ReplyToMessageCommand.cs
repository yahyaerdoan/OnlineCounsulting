using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Rules;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;

public record ReplyToMessageCommand(Guid MessageId, string ReplyBody) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MessagesOperationClaims.Admin, MessagesOperationClaims.Update];
}

public class ReplyToMessageHandler(IMessageRepository repository, IEmailOutboxWriter<IInquiriesOutboxModule> outboxWriter, IEmailTemplate<MessageReplyEmailModel> replyTemplate)
    : IRequestHandler<ReplyToMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ReplyToMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await repository.GetAsync(m => m.Id == request.MessageId, cancellationToken: cancellationToken);
        if (message is null)
        {
            return MessageBusinessRules.MessageNotFound(request.MessageId);
        }

        message.RepliedAt = DateTimeOffset.UtcNow;
        _ = await repository.UpdateAsync(message);

        var model = new MessageReplyEmailModel(message.FirstName, message.Subject, request.ReplyBody);
        var sourceReference = $"Message:{message.Id}";

        await outboxWriter.EnqueueAsync(message.Email, replyTemplate.Subject(model), replyTemplate.Build(model), sourceReference: sourceReference, cancellationToken: cancellationToken);

        return Result.Success("Reply sent successfully.");
    }
}
