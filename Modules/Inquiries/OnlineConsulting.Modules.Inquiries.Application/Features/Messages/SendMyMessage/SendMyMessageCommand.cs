using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SendMyMessage;

/// <summary>Starts a conversation from the customer's inbox; name and email come from their account.</summary>
public record SendMyMessageCommand(Guid UserId, string Subject, string Description) : IRequest<OperationDataResult<MessageResponse>>, IInquiriesTransactionRequest;

public class SendMyMessageHandler(IUserContactReader contactReader, MessageSubmitter submitter) : IRequestHandler<SendMyMessageCommand, OperationDataResult<MessageResponse>>
{
    public async Task<OperationDataResult<MessageResponse>> Handle(SendMyMessageCommand request, CancellationToken cancellationToken)
    {
        if (await contactReader.GetContactAsync(request.UserId, cancellationToken) is not { Email: { } email } contact)
        {
            return Result.NotFound<MessageResponse>(MessageMessages.SenderNotFound);
        }

        var message = new Message
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = email,
            Subject = request.Subject.Trim(),
            Description = request.Description.Trim(),
            UserId = request.UserId,
        };

        await submitter.SubmitAsync(message, cancellationToken);

        return Result.Created(MessageResponse.FromDomain(message), MessageMessages.MessageSubmitted);
    }
}
