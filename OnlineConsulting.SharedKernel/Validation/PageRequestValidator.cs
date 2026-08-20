using Core.ApplicationLayer.Requests.Page;
using FluentValidation;

namespace OnlineConsulting.SharedKernel.Validation;

/// <summary>Shared via SetValidator() from each paged query's own validator; caps PageSize so a caller can't force the whole table through the query.</summary>
public class PageRequestValidator : AbstractValidator<PageRequest>
{
    public const int MaxPageSize = 100;

    public PageRequestValidator()
    {
        _ = RuleFor(p => p.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index must be {ComparisonValue} or greater.");
        _ = RuleFor(p => p.PageSize).InclusiveBetween(1, MaxPageSize).WithMessage("Page size must be between {From} and {To}.");
    }
}
