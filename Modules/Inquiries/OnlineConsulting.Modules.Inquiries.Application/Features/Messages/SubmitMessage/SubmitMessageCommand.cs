using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

public record SubmitMessageCommand(string FirstName, string LastName, string Email, string Subject, string Description) : IRequest<OperationResult>, ITransactionAddRequest;

public class SubmitMessageHandler(IMessageRepository repository, IEmailOutboxWriter<IInquiriesOutboxModule> outboxWriter, IEmailTemplate<MessageReceivedEmailModel> receivedTemplate, IEmailTemplate<NewInquiryNotificationEmailModel> notificationTemplate, IOptions<InquiriesOptions> options)
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

        _ = await repository.AddAsync(message);

        var receivedModel = new MessageReceivedEmailModel(request.FirstName, request.Subject);

        await outboxWriter.EnqueueAsync(request.Email, receivedTemplate.Subject(receivedModel), receivedTemplate.Build(receivedModel), sourceReference: sourceReference, cancellationToken: cancellationToken);

        var notificationModel = new NewInquiryNotificationEmailModel(request.FirstName, request.LastName, request.Email, request.Subject, request.Description);

        await outboxWriter.EnqueueAsync(options.Value.AdminNotificationEmail, notificationTemplate.Subject(notificationModel), notificationTemplate.Build(notificationModel), sourceReference: sourceReference, cancellationToken: cancellationToken);

        return Result.Created("Message submitted successfully.");
    }
}
