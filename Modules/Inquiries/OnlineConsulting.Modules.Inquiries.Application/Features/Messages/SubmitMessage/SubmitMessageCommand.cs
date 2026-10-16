using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

/// <summary>Public contact form; a signed-in sender's message also shows in their inbox.</summary>
public record SubmitMessageCommand(string FirstName, string LastName, string Email, string Subject, string Description, Guid? UserId = null) : IRequest<OperationResult>, IInquiriesTransactionRequest;

public class SubmitMessageHandler(IMessageRepository repository, MessageSubmitter submitter) : IRequestHandler<SubmitMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SubmitMessageCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var subject = request.Subject.Trim();
        var description = request.Description.Trim();

        // A guest writing again continues their existing conversation.
        if (request.UserId is null && await repository.GetLatestGuestConversationAsync(email, cancellationToken) is { } conversation)
        {
            var body = string.Equals(subject, conversation.Subject, StringComparison.OrdinalIgnoreCase) ? description : $"{subject}\n\n{description}";
            await submitter.ContinueGuestAsync(conversation, subject, body, cancellationToken);

            return Result.Created(MessageMessages.MessageSubmitted);
        }

        await submitter.SubmitAsync(new Message
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Subject = subject,
            Description = description,
            UserId = request.UserId,
        }, cancellationToken);

        return Result.Created(MessageMessages.MessageSubmitted);
    }
}
