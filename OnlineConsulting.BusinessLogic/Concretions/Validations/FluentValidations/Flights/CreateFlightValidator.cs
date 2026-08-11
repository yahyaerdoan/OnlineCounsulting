using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using System.Globalization;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Flights;

public class CreateFlightValidator : AbstractValidator<CreateFlightDto>
{
    public CreateFlightValidator()
    {
        RuleFor(x => x.Origin)
           .NotEmpty().WithMessage("Origin is required.")
           .Length(3).WithMessage("Origin must be exactly 3 characters.")
           .Matches("^[A-Z]{3}$").WithMessage("Origin must be uppercase IATA code.");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Destination is required.")
            .Length(3).WithMessage("Destination must be exactly 3 characters.")
            .Matches("^[A-Z]{3}$").WithMessage("Destination must be uppercase IATA code.");

        RuleFor(x => x.PlannedDeparture)
            .NotEmpty().WithMessage("Planned departure time is required.")
            .Must(BeValidDate).WithMessage("Planned departure must be a valid date.")
            .Must(BeInTheFuture).WithMessage("Planned departure must be in the future.");

        RuleFor(x => x.PlannedArrival)
            .NotEmpty().WithMessage("Planned arrival time is required.")
            .Must(BeValidDate).WithMessage("Planned arrival must be a valid date.")
            .Must((dto, arrival) => BeAfterDeparture(dto.PlannedDeparture, arrival))
            .WithMessage("Planned arrival must be after departure.");
    }

    private bool BeValidDate(string date) => DateTime.TryParse(date, out _);

    private bool BeInTheFuture(string date)
    {
        return DateTime.TryParse(date, null, DateTimeStyles.AdjustToUniversal, out var parsedDate)
          && parsedDate.ToUniversalTime() > DateTime.UtcNow;
    }

    private static bool BeAfterDeparture(string departure, string arrival) => DateTime.TryParse(departure, out var depDate) && DateTime.TryParse(arrival, out var arrDate) && arrDate > depDate;
}
