namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/site-content/legal-policy.</summary>
public record LegalPolicyResponse(string? PrivacyPolicy, string? TermsOfService, DateTimeOffset UpdatedDate);
