namespace OnlineConsulting.Modules.Scheduling.Application.Common;

public static class SchedulingOperationClaims
{
    public const string Admin = "scheduling.admin";
    public const string Write = "scheduling.write";

    public static readonly string[] All = [Admin, Write];
}
