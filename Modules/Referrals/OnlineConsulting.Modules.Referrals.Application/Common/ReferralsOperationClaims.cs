namespace OnlineConsulting.Modules.Referrals.Application.Common;

public static class ReferralsOperationClaims
{
    public const string Admin = "referrals.admin";
    public const string Read = "referrals.read";
    public const string Write = "referrals.write";
    public const string Add = "referrals.add";
    public const string Update = "referrals.update";
    public const string Delete = "referrals.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
