using FluentValidation;
using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.GetServicesByCategory;

public class GetServicesByCategoryValidator : AbstractValidator<GetServicesByCategoryQuery>
{
    public GetServicesByCategoryValidator()
    {
        _ = RuleFor(x => x.CategoryId).NotEmpty();
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
