using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetSubscribers;

public class GetSubscribersValidator : AbstractValidator<GetSubscribersQuery>
{
    public GetSubscribersValidator()
    {
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
