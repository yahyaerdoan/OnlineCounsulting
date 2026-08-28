namespace OnlineConsulting.Modules.Scheduling.Application.Common;

public static class SchedulingOperationClaims
{
    public const string Admin = "scheduling.admin";
    public const string Read = "scheduling.read";
    public const string Write = "scheduling.write";
    public const string Add = "scheduling.add";
    public const string Update = "scheduling.update";
    public const string Delete = "scheduling.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
