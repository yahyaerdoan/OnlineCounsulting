using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class FlightRepository(OnlineConsultingDbContext context) : GenericRepository<Flight>(context), IFlightRepository
{
    public IQueryable<Flight> GetFlightsByAirportAndDay(string airportCode, DateTime date, bool tracking = true, bool? status = true)
    {
        IQueryable<Flight> query = Entity;

        if (status.HasValue)
            query = query.Where(f => f.Status == status.Value);

        airportCode = airportCode.Trim().ToUpper();

        query = query.Where(f =>
            (f.Origin == airportCode || f.Destination == airportCode) &&
            f.PlannedDeparture.Date == date.Date).OrderBy(f => f.PlannedDeparture);

        if (!tracking)
            query = query.AsNoTracking();

        return query;
    }
}
