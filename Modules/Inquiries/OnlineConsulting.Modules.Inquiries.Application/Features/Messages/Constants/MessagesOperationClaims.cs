namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;

public static class MessagesOperationClaims
{
    public const string Admin = "messages.admin";
    public const string Read = "messages.read";
    public const string Delete = "messages.delete";

    public static readonly string[] All = [Admin, Read, Delete];
}
