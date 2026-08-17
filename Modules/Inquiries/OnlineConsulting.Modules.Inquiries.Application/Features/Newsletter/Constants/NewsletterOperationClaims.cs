namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;

public static class NewsletterOperationClaims
{
    public const string Admin = "newsletter.admin";
    public const string Read = "newsletter.read";
    public const string Delete = "newsletter.delete";

    public static readonly string[] All = [Admin, Read, Delete];
}
