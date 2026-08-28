namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Constants;

public static class ContactOperationClaims
{
    public const string Admin = "contact.admin";
    public const string Read = "contact.read";
    public const string Write = "contact.write";
    public const string Add = "contact.add";
    public const string Update = "contact.update";
    public const string Delete = "contact.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
