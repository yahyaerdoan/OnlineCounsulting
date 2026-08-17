namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;

public static class UsersOperationClaims
{
    public const string Admin = "users.admin";
    public const string Read = "users.read";
    public const string Write = "users.write";
    public const string Update = "users.update";
    public const string Delete = "users.delete";

    public static readonly string[] All = [Admin, Read, Write, Update, Delete];
}
