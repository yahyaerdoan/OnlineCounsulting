using FluentValidation;
using OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.Constants;

namespace OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RegisterDeviceToken;

public class RegisterDeviceTokenValidator : AbstractValidator<RegisterDeviceTokenCommand>
{
    public RegisterDeviceTokenValidator()
    {
        _ = RuleFor(x => x.Token).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.Platform).NotEmpty().Must(p => DevicePlatforms.All.Contains(p))
            .WithMessage($"Platform must be one of: {string.Join(", ", DevicePlatforms.All)}.");
    }
}
