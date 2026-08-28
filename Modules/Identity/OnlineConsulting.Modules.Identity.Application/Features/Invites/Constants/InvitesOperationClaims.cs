namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;

public static class InvitesOperationClaims
{
    public const string Admin = "invites.admin";
    public const string Read = "invites.read";
    public const string Write = "invites.write";
    public const string Add = "invites.add";
    public const string Update = "invites.update";
    public const string Delete = "invites.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
