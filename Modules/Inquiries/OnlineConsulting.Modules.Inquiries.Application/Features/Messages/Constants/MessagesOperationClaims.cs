namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;

public static class MessagesOperationClaims
{
    public const string Admin = "messages.admin";
    public const string Read = "messages.read";
    public const string Write = "messages.write";
    public const string Add = "messages.add";
    public const string Update = "messages.update";
    public const string Delete = "messages.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
