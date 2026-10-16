using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Rules;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMyMessage;

/// <summary>Customer's follow-up; puts the conversation back to awaiting reply and emails staff.</summary>
public record ReplyToMyMessageCommand(Guid UserId, Guid MessageId, string ReplyBody) : IRequest<OperationResult>, IInquiriesTransactionRequest;

public class ReplyToMyMessageHandler(IMessageRepository repository, MessageSubmitter submitter) : IRequestHandler<ReplyToMyMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ReplyToMyMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await repository.GetAsync(m => m.Id == request.MessageId && m.UserId == request.UserId, cancellationToken: cancellationToken);
        if (message is null)
        {
            return MessageBusinessRules.MessageNotFound(request.MessageId);
        }

        await submitter.AddCustomerReplyAsync(message, request.ReplyBody.Trim(), cancellationToken);

        return Result.Success(MessageMessages.ReplySent);
    }
}
