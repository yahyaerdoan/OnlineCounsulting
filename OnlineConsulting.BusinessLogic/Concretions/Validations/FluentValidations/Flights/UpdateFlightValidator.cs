using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using System.Globalization;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Flights;

public class UpdateFlightValidator : AbstractValidator<UpdateFlightDto>
{
    public UpdateFlightValidator()
    {
        RuleFor(x => x.PlannedDeparture)
           .NotEmpty().WithMessage("If provided, Planned departure time is required.")
           .Must(BeValidDate).WithMessage("If provided, Planned departure must be a valid date.")
           .Must(BeInTheFuture).WithMessage("If provided, Planned Departure must be in the future.")
           .When(x => x.PlannedDeparture is not null);

        RuleFor(x => x.PlannedArrival)
           .NotEmpty().WithMessage("If provided, Planned arrival time is required.")
           .Must(BeValidDate).WithMessage("If provided, Planned arrival must be a valid date.")
           .Must((dto, arrival) => BeAfterDeparture(dto.PlannedDeparture, arrival))
           .WithMessage("If provided, Planned Arrival must be in the future.")
           .When(x => x.PlannedArrival is not null);

        RuleFor(x => x.Gate)
           .NotEmpty().WithMessage("If provided, Gate cannot be empty.")
           .MaximumLength(3)
           .WithMessage("If provided, Gate can be max 3 characters.")
           .When(x => x.Gate is not null);
    }

    private bool BeValidDate(string? date) => date is not null && DateTime.TryParse(date, out _);

    private bool BeInTheFuture(string? date) => DateTime.TryParse(date, null, DateTimeStyles.AdjustToUniversal, out var parsedDate) && parsedDate.ToUniversalTime() > DateTime.UtcNow;

    private static bool BeAfterDeparture(string? departure, string? arrival) => departure is not null && arrival is not null && DateTime.TryParse(departure, out var depDate) && DateTime.TryParse(arrival, out var arrDate) && arrDate > depDate;
}
