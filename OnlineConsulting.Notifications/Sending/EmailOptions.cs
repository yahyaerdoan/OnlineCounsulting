namespace OnlineConsulting.Notifications.Sending;

public class EmailOptions
{
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromAddress { get; set; }
    public required string FromName { get; set; }
    public bool UseSsl { get; set; } = true;
}
