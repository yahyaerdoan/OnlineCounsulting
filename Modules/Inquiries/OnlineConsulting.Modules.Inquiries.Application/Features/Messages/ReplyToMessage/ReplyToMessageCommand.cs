using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Rules;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;

public record ReplyToMessageCommand(Guid MessageId, string ReplyBody) : IRequest<OperationResult>, ISecureAddRequest
{
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

        var now = DateTimeOffset.UtcNow;
        message.RepliedAt = now;
        message.LastMessageAt = now;
        await repository.AddReplyAsync(message, new MessageReply { MessageId = message.Id, Body = request.ReplyBody.Trim() }, cancellationToken);

        var model = new MessageReplyEmailModel(message.FirstName, message.Subject, request.ReplyBody);
        var sourceReference = MessageSubmitter.SourceReference(message);

        await outboxWriter.EnqueueAsync(message.Email, replyTemplate.Subject(model), replyTemplate.Build(model), sourceReference: sourceReference, cancellationToken: cancellationToken);

        return Result.Success(MessageMessages.ReplySent);
    }
}
