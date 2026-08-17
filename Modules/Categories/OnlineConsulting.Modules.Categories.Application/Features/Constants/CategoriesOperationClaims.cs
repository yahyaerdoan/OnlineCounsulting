namespace OnlineConsulting.Modules.Categories.Application.Features.Constants;

public static class CategoriesOperationClaims
{
    public const string Admin = "categories.admin";
    public const string Read = "categories.read";
    public const string Write = "categories.write";
    public const string Add = "categories.add";
    public const string Update = "categories.update";
    public const string Delete = "categories.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
