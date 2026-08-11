namespace OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;

public class AppSettingStripeOption
{
    public const string StripeKeys = "Stripe";
    public string Domain { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
