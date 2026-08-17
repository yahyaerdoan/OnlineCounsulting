namespace OnlineConsulting.UserInterface.Features.Account;

/// <summary>UI-local replacement for the legacy OnlineConsulting.BusinessLogic AppSettingRecaptchaOption. Bound
/// from the same "Recaptcha" config section.</summary>
public class RecaptchaOptions
{
    public const string SectionName = "Recaptcha";
    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
