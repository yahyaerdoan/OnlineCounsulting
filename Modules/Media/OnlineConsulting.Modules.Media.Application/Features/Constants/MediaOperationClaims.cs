namespace OnlineConsulting.Modules.Media.Application.Features.Constants;

public static class MediaOperationClaims
{
    public const string Admin = "media.admin";
    public const string Read = "media.read";
    public const string Write = "media.write";
    public const string Add = "media.add";
    public const string Update = "media.update";
    public const string Delete = "media.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
