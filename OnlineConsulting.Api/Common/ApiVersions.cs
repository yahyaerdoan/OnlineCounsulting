using Asp.Versioning;

namespace OnlineConsulting.Api.Common;

/// <summary>The API versions this build serves; routes are /api/v{major}/... and a new version is added only for a breaking change.</summary>
public static class ApiVersions
{
    public static readonly ApiVersion V1 = new(1, 0);
}
