namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages;

/// <summary>Which mailbox gets notified of new inquiries, kept separate from EmailOptions which only knows how to send.</summary>
public class InquiriesOptions
{
    public required string AdminNotificationEmail { get; set; }
}
