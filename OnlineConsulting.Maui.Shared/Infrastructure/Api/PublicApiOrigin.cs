namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>The Api origin a browser can reach, registered by hosts whose server-side HttpClient uses an address browsers cannot resolve (e.g. Aspire service discovery "https+http://api").</summary>
public sealed record PublicApiOrigin(Uri BaseAddress);
