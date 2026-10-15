using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.UpdateLandingBlock;

public class UpdateLandingBlockValidator : AbstractValidator<UpdateLandingBlockCommand>
{
    public UpdateLandingBlockValidator() => Include(new LandingBlockFieldsValidator());
}
