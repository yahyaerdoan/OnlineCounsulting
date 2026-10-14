using OnlineConsulting.SharedKernel.Authorization;

namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Landing content sells the platform, so only SuperAdmin edits it.</summary>
public static class LandingAccess
{
    public static readonly string[] Roles = [GlobalOperationClaims.SuperAdmin];
}
