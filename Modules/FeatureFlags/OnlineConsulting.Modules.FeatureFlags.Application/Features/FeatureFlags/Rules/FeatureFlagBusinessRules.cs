using OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags.Rules;

public static class FeatureFlagBusinessRules
{
    public static OperationResult UnknownKey(string key) =>
        Result.NotFound(string.Format(FeatureFlagMessages.UnknownKeyFormat, key));
}
