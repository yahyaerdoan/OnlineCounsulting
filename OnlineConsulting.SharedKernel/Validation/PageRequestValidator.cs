using Core.ApplicationLayer.Requests.Page;
using FluentValidation;

namespace OnlineConsulting.SharedKernel.Validation;

// Shared via SetValidator() from each paged query's own validator - PageRequest itself is never
// sent as a MediatR request, so it isn't picked up by the validation pipeline on its own. Caps
// PageSize so a caller can't request an unbounded page (e.g. 1_000_000) and force the whole table
// through the query.
public class PageRequestValidator : AbstractValidator<PageRequest>
{
    public const int MaxPageSize = 100;

    public PageRequestValidator()
    {
        RuleFor(p => p.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index must be {ComparisonValue} or greater.");
        RuleFor(p => p.PageSize).InclusiveBetween(1, MaxPageSize).WithMessage("Page size must be between {From} and {To}.");
    }
}
