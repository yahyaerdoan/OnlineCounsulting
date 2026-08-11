namespace OnlineConsulting.DataAccess.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;

public class AppSettingOption
{
    public const string DbConnection = "OnlineConsultingDbConnections";
    public string DevelopmentDbConnection { get; set; } = string.Empty;
    public string LiveDbConnection { get; set; } = string.Empty;
}
