using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetSubscribers;

public class GetSubscribersValidator : AbstractValidator<GetSubscribersQuery>
{
    public GetSubscribersValidator()
    {
        RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
