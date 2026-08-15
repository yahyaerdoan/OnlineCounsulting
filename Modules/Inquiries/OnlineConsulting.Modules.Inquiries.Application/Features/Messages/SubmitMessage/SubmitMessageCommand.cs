using MediatR;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

// Public - anyone can submit a contact form, no login required. Not ITransactionAddRequest: the
// outbox rows are staged via IEmailOutboxWriter.Enqueue BEFORE the repository call, so
// IMessageRepository.AddAsync's own SaveChangesAsync commits the message and both outbox rows
// together in one call - already atomic without an extra transaction wrapper.
public record SubmitMessageCommand(string FirstName, string LastName, string Email, string Subject, string Description) : IRequest<OperationResult>;

public class SubmitMessageHandler(IMessageRepository repository, IEmailOutboxWriter outboxWriter, IEmailTemplate<MessageReceivedEmailModel> receivedTemplate, IEmailTemplate<NewInquiryNotificationEmailModel> notificationTemplate, IOptions<InquiriesOptions> options)
    : IRequestHandler<SubmitMessageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SubmitMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Subject = request.Subject,
            Description = request.Description,
        };
        var sourceReference = $"Message:{message.Id}";

        var receivedModel = new MessageReceivedEmailModel(request.FirstName, request.Subject);
        outboxWriter.Enqueue(request.Email, receivedTemplate.Subject(receivedModel), receivedTemplate.Build(receivedModel),
            sourceReference: sourceReference);

        var notificationModel = new NewInquiryNotificationEmailModel(request.FirstName, request.LastName, request.Email, request.Subject, request.Description);
        outboxWriter.Enqueue(options.Value.AdminNotificationEmail, notificationTemplate.Subject(notificationModel), notificationTemplate.Build(notificationModel),
            sourceReference: sourceReference);

        await repository.AddAsync(message);

        return Result.Created("Message submitted successfully.");
    }
}
