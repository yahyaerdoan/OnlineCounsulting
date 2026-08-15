namespace OnlineConsulting.Modules.Inquiries.Application;

// Which mailbox gets notified of new inquiries - a business/config concern of this module, kept
// separate from OnlineConsulting.Notifications' EmailOptions (which only knows HOW to send, not
// WHO should be told about a new contact-form submission).
public class InquiriesOptions
{
    public required string AdminNotificationEmail { get; set; }
}
