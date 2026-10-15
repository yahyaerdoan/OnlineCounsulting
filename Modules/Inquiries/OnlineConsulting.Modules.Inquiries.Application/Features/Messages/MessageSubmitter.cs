using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages;

/// <summary>Saves new messages and emails the sender and staff; shared by the contact form and the customer inbox.</summary>
public class MessageSubmitter(IMessageRepository repository, IEmailOutboxWriter<IInquiriesOutboxModule> outboxWriter, IEmailTemplate<MessageReceivedEmailModel> receivedTemplate,
    IEmailTemplate<NewInquiryNotificationEmailModel> notificationTemplate, IOptions<InquiriesOptions> options)
{
    public async Task SubmitAsync(Message message, CancellationToken cancellationToken)
    {
        _ = await repository.AddAsync(message, cancellationToken: cancellationToken);

        var receivedModel = new MessageReceivedEmailModel(message.FirstName, message.Subject);
        await outboxWriter.EnqueueAsync(message.Email, receivedTemplate.Subject(receivedModel), receivedTemplate.Build(receivedModel), sourceReference: SourceReference(message), cancellationToken: cancellationToken);

        await NotifyStaffAsync(message, message.Subject, message.Description, cancellationToken);
    }

    /// <summary>Emails staff a new message or a customer's follow-up.</summary>
    public async Task NotifyStaffAsync(Message message, string subject, string body, CancellationToken cancellationToken)
    {
        var model = new NewInquiryNotificationEmailModel(message.FirstName, message.LastName, message.Email, subject, body);
        await outboxWriter.EnqueueAsync(options.Value.AdminNotificationEmail, notificationTemplate.Subject(model), notificationTemplate.Build(model), sourceReference: SourceReference(message), cancellationToken: cancellationToken);
    }

    public static string SourceReference(Message message) => $"Message:{message.Id}";
}
