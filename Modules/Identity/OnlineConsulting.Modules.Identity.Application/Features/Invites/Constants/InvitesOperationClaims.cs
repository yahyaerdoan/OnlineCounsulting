namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;

public static class InvitesOperationClaims
{
    public const string Admin = "invites.admin";
    public const string Read = "invites.read";
    public const string Add = "invites.add";

    public static readonly string[] All = [Admin, Read, Add];
}
