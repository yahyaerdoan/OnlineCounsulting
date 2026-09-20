namespace OnlineConsulting.Modules.Commerce.Application.Common;

public static class CommerceOperationClaims
{
    public const string Admin = "commerce.admin";
    public const string Read = "commerce.read";
    public const string Write = "commerce.write";
    public const string Add = "commerce.add";
    public const string Update = "commerce.update";
    public const string Delete = "commerce.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
