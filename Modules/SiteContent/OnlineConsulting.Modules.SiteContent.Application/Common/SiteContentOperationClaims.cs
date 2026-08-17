namespace OnlineConsulting.Modules.SiteContent.Application.Common;

public static class SiteContentOperationClaims
{
    public const string Admin = "sitecontent.admin";
    public const string Write = "sitecontent.write";

    public static readonly string[] All = [Admin, Write];
}
