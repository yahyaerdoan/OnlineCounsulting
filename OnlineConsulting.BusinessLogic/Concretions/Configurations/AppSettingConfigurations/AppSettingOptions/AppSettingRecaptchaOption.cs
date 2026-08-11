namespace OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;

public class AppSettingRecaptchaOption
{
    public const string RecaptchaKeys = "Recaptcha";
    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
