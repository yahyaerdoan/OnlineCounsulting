namespace OnlineConsulting.Modules.Equipment.Application.Common;

public static class EquipmentOperationClaims
{
    public const string Admin = "equipment.admin";
    public const string Read = "equipment.read";
    public const string Write = "equipment.write";
    public const string Add = "equipment.add";
    public const string Update = "equipment.update";
    public const string Delete = "equipment.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
