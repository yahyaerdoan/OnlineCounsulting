using OnlineConsulting.Modules.Identity.Application.Features.Auth.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Rules;

public static class AuthBusinessRules
{
    public static OperationResult InvalidCredentials() => Result.BadRequest(AuthMessages.InvalidCredentials);

    /// <summary>The platform has no storefront; its accounts are staff and business owners only.</summary>
    public static OperationResult PlatformRegistrationClosed() => Result.BadRequest(AuthMessages.PlatformRegistrationClosed);
}
