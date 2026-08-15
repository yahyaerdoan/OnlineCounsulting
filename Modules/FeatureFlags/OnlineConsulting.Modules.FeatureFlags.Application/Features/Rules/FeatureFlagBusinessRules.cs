using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.Rules;

public static class FeatureFlagBusinessRules
{
    public static OperationResult UnknownKey(string key) =>
        Result.BadRequest(string.Format(FeatureFlagMessages.UnknownKeyFormat, key));
}
