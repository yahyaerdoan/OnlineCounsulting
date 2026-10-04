using Hateoas;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>The IANA zone the business runs in; clients show every date and time in it.</summary>
public record BusinessTimeZoneResponse(string TimeZoneId) : LinkedRecord;
