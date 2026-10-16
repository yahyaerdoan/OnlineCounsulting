using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Inquiries;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages;

/// <summary>Saves new messages and emails the sender and staff; shared by the contact form and the customer inbox.</summary>
public class MessageSubmitter(IMessageRepository repository, IEmailOutboxWriter<IInquiriesOutboxModule> outboxWriter, IEmailTemplate<MessageReceivedEmailModel> receivedTemplate,
    IEmailTemplate<NewInquiryNotificationEmailModel> notificationTemplate, IBusinessContactReader businessContactReader, ITenantContactReader tenantContactReader)
{
    public async Task SubmitAsync(Message message, CancellationToken cancellationToken)
    {
        _ = await repository.AddAsync(message, cancellationToken: cancellationToken);

        await ConfirmToSenderAsync(message, message.Subject, cancellationToken);
        await NotifyStaffAsync(message, message.Subject, message.Description, cancellationToken);
    }

    /// <summary>Guest wrote again with the same email: adds it to their conversation.</summary>
    public async Task ContinueGuestAsync(Message conversation, string subject, string body, CancellationToken cancellationToken)
    {
        await AddCustomerReplyAsync(conversation, body, cancellationToken);
        await ConfirmToSenderAsync(conversation, subject, cancellationToken);
    }

    /// <summary>Adds the customer's follow-up to the conversation, reopens it and emails staff.</summary>
    public async Task AddCustomerReplyAsync(Message message, string body, CancellationToken cancellationToken)
    {
        message.RepliedAt = null;
        message.LastMessageAt = DateTimeOffset.UtcNow;
        await repository.AddReplyAsync(message, new MessageReply { MessageId = message.Id, Body = body, IsFromCustomer = true }, cancellationToken);

        await NotifyStaffAsync(message, $"Re: {message.Subject}", body, cancellationToken);
    }

    /// <summary>Emails the business a new message or a customer's follow-up.</summary>
    public async Task NotifyStaffAsync(Message message, string subject, string body, CancellationToken cancellationToken)
    {
        var staffEmail = await StaffEmailAsync(message.TenantId, cancellationToken);
        if (string.IsNullOrWhiteSpace(staffEmail))
        {
            return;
        }

        var model = new NewInquiryNotificationEmailModel(message.FirstName, message.LastName, message.Email, subject, body);
        await outboxWriter.EnqueueAsync(staffEmail, notificationTemplate.Subject(model), notificationTemplate.Build(model), sourceReference: SourceReference(message), cancellationToken: cancellationToken);
    }

    /// <summary>Contact page email, else the email the business signed up with.</summary>
    private async Task<string?> StaffEmailAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var contact = await businessContactReader.GetAsync(tenantId, cancellationToken);
        return string.IsNullOrWhiteSpace(contact?.Email) ? await tenantContactReader.GetContactEmailAsync(tenantId, cancellationToken) : contact.Email;
    }

    private async Task ConfirmToSenderAsync(Message message, string subject, CancellationToken cancellationToken)
    {
        var model = new MessageReceivedEmailModel(message.FirstName, subject);
        await outboxWriter.EnqueueAsync(message.Email, receivedTemplate.Subject(model), receivedTemplate.Build(model), sourceReference: SourceReference(message), cancellationToken: cancellationToken);
    }

    public static string SourceReference(Message message) => $"Message:{message.Id}";
}
