namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;

/// <summary>The admin fields a tenant signup collects - shared so the same rules run before the card is charged (ValidateTenantAdminQuery) and when the user is created.</summary>
public interface ITenantAdminFields
{
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string Password { get; }
}
