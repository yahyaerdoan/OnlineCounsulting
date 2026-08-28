namespace OnlineConsulting.Modules.SiteContent.Application.Common;

public static class SiteContentOperationClaims
{
    public const string Admin = "sitecontent.admin";
    public const string Read = "sitecontent.read";
    public const string Write = "sitecontent.write";
    public const string Add = "sitecontent.add";
    public const string Update = "sitecontent.update";
    public const string Delete = "sitecontent.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
