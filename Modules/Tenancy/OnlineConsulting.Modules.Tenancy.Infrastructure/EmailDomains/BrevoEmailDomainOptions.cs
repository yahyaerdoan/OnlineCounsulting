namespace OnlineConsulting.Modules.Tenancy.Infrastructure.EmailDomains;

/// <summary>Bound from the Email:Brevo config section, the same account that sends the mail.</summary>
public class BrevoEmailDomainOptions
{
    public string ApiUrl { get; set; } = "https://api.brevo.com/v3";
    public string ApiKey { get; set; } = string.Empty;
}
