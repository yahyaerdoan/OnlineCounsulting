using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class FlightManager(IMapper mapper, IGenericRepository<Flight> repository, IFlightRepository flightRepository) : GenericService<Flight, IDto>(mapper, repository), IFlightService
{
    public async Task<IOperationResult> AddFlightToScheduleAsync(CreateFlightDto createFlightDto)
    {
        var newFlight = _mapper.Map<Flight>(createFlightDto);

        newFlight.FlightNumber = GenerateFlightNumber();
        newFlight.FlightStatus = FlightStatus.Scheduled;

        var isAdded = await _repository.AddAsync(newFlight);
        if (!isAdded)
            return new ErrorResult("Failed to add flight to the database.", ResultStatus.BadRequest);

        var rowsAffected = await _repository.SaveAsync();
        if (rowsAffected <= 0)
            return new ErrorResult("Failed to persist flight to the database.", ResultStatus.BadRequest);

        return new SuccessResult("Flight successfully scheduled.", ResultStatus.Created);
    }
    public async Task<IOperationResult<List<TDto>>> GetFlightsByAirportGivenDayAsync<TDto>(string airportCode, DateTime date, bool tracking = true, bool? status = true)
    {
        var flightsQuery = flightRepository.GetFlightsByAirportAndDay(airportCode, date, tracking, status);

        var flightList = await flightsQuery.ToListAsync();

        if (flightList is null || flightList.Count == 0)
            return new ErrorDataResult<List<TDto>>("No flights found for the specified airport and date.", ResultStatus.NotFound);

        foreach (var flight in flightList)
        {
            flight.FlightStatus = FlightStatusCalculator.ComputeStatus(flight);
        }

        var flightDtos = _mapper.Map<List<TDto>>(flightList);
        return new SuccessDataResult<List<TDto>>(flightDtos, "Flights retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateFlightDepartureAndGateAsync(UpdateFlightDto updateFlightDto)
    {
        var flightResult = await GetByIdAsync<Flight>(updateFlightDto.Id.ToString());

        if (!flightResult.IsSuccessful || flightResult.Data is null)
            return new ErrorResult(flightResult.Title, flightResult.Status);

        var flightEntity = flightResult.Data;

        ApplyFlightUpdates(flightEntity, updateFlightDto);

        await _repository.UpdateAsync(flightEntity);
        await SaveAsync();

        return new SuccessResult("Flight updated successfully.", ResultStatus.Ok);
    }
    private static void ApplyFlightUpdates(Flight flightEntity, UpdateFlightDto updateDto)
    {
        if (!string.IsNullOrEmpty(updateDto.PlannedDeparture))
            flightEntity.PlannedDeparture = DateTime.Parse(updateDto.PlannedDeparture);

        if (!string.IsNullOrEmpty(updateDto.PlannedArrival))
            flightEntity.PlannedArrival = DateTime.Parse(updateDto.PlannedArrival);

        if (!string.IsNullOrEmpty(updateDto.Gate))
            flightEntity.Gate = updateDto.Gate;
    }
    private static string GenerateFlightNumber() => $"TK-{Random.Shared.Next(1000, 9999)}";
}
public static class FlightStatusCalculator
{
    public static FlightStatus ComputeStatus(Flight flight, DateTime? now = null)
    {
        var currentTime = now ?? DateTime.Now;

        if (flight.CancelledAt.HasValue)
            return FlightStatus.Cancelled;

        if (flight.ActualArrival.HasValue)
            return FlightStatus.Arrived;

        if (flight.ActualDeparture.HasValue)
            return FlightStatus.Departed;

        if (currentTime < flight.PlannedDeparture.AddMinutes(-30))
            return FlightStatus.Scheduled;

        if (currentTime >= flight.PlannedDeparture.AddMinutes(-30) && currentTime < flight.PlannedDeparture)
            return FlightStatus.Boarding;

        if (currentTime >= flight.PlannedDeparture && currentTime < flight.PlannedArrival)
            return FlightStatus.Departed;

        if (currentTime >= flight.PlannedArrival)
            return FlightStatus.Arrived;

        return FlightStatus.Scheduled;
    }
}
