namespace OnlineConsulting.Modules.Services.Application.Features.Constants;

public static class ServicesOperationClaims
{
    public const string Admin = "services.admin";
    public const string Read = "services.read";
    public const string Write = "services.write";
    public const string Add = "services.add";
    public const string Update = "services.update";
    public const string Delete = "services.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
