using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IFlightService : IGenericService<Flight, IDto>
{
    Task<IOperationResult> AddFlightToScheduleAsync(CreateFlightDto createFlightDto);
    Task<IOperationResult> UpdateFlightDepartureAndGateAsync(UpdateFlightDto updateFlightDto);
    Task<IOperationResult<List<TDto>>> GetFlightsByAirportGivenDayAsync<TDto>(string airportCode, DateTime date, bool tracking = true, bool? status = true);
}
