namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Accept-invite form model. No DataAnnotations - the API's FluentValidation rules are the source of truth.</summary>
public class AcceptInviteModel
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
