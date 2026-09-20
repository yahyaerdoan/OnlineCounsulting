namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/contact's response shape.</summary>
public record CompanyContactResponse(Guid Id, string Email, string Phone, string Address, string Description, string WorkingHours);
