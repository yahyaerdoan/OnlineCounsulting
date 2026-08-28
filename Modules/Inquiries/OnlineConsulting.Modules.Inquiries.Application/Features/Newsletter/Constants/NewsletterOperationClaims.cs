namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;

public static class NewsletterOperationClaims
{
    public const string Admin = "newsletter.admin";
    public const string Read = "newsletter.read";
    public const string Write = "newsletter.write";
    public const string Add = "newsletter.add";
    public const string Update = "newsletter.update";
    public const string Delete = "newsletter.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
