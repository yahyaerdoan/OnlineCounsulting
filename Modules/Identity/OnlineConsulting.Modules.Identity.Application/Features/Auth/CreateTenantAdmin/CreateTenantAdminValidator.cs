namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>No email-uniqueness check here (unlike RegisterValidator) - UserManager.CreateAsync's atomic unique index is the real guard, avoiding a check-then-act race.</summary>
public class CreateTenantAdminValidator : TenantAdminFieldsValidator<CreateTenantAdminCommand>
{
}
