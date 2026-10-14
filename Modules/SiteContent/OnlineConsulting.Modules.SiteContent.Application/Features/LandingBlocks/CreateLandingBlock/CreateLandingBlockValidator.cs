using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.CreateLandingBlock;

public class CreateLandingBlockValidator : AbstractValidator<CreateLandingBlockCommand>
{
    public CreateLandingBlockValidator() => Include(new LandingBlockFieldsValidator());
}
