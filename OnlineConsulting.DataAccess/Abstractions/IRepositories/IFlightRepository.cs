using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IFlightRepository
{
    IQueryable<Flight> GetFlightsByAirportAndDay(string airportCode, DateTime date, bool traking = true, bool? status = true);
}
