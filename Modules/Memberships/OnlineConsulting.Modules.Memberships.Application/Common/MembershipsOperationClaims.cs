namespace OnlineConsulting.Modules.Memberships.Application.Common;

public static class MembershipsOperationClaims
{
    public const string Admin = "memberships.admin";
    public const string Read = "memberships.read";
    public const string Write = "memberships.write";
    public const string Add = "memberships.add";
    public const string Update = "memberships.update";
    public const string Delete = "memberships.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
