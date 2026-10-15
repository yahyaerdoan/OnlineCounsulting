using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

/// <summary>Public contact form; a signed-in sender's message also shows in their inbox.</summary>
public record SubmitMessageCommand(string FirstName, string LastName, string Email, string Subject, string Description, Guid? UserId = null) : IRequest<OperationResult>, IInquiriesTransactionRequest;

public class SubmitMessageHandler(MessageSubmitter submitter) : IRequestHandler<SubmitMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SubmitMessageCommand request, CancellationToken cancellationToken)
    {
        await submitter.SubmitAsync(new Message
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Subject = request.Subject,
            Description = request.Description,
            UserId = request.UserId,
        }, cancellationToken);

        return Result.Created(MessageMessages.MessageSubmitted);
    }
}
