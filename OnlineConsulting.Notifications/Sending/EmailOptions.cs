namespace OnlineConsulting.Notifications.Sending;

/// <summary>Bound from the "Email" config section; From* apply to every provider, Smtp* only to the Smtp one.</summary>
public class EmailOptions
{
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromAddress { get; set; }
    public required string FromName { get; set; }
    public bool UseSsl { get; set; } = true;
    public BrevoEmailOptions Brevo { get; set; } = new();
}

/// <summary>Brevo transactional email API settings.</summary>
public class BrevoEmailOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
