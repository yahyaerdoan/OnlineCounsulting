namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Constants;

public static class ContactOperationClaims
{
    public const string Admin = "contact.admin";
    public const string Write = "contact.write";

    public static readonly string[] All = [Admin, Write];
}
