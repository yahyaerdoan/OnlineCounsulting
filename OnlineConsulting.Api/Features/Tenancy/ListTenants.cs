using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ListTenants;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class ListTenants : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/admin/tenants/query", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ListTenants")
            .WithDescription("Returns every tenant on the platform, paginated (?index=&size=), optionally narrowed to one status (?status=) and filtered/sorted via a DynamicQuery body, with active module and pricing summary (SuperAdmin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery,
        [FromQuery] string? status = null)
    {
        var result = await sender.Send(new ListTenantsQuery(query.ToPageRequest(), dynamicQuery, status));
        return result.ToEnvelopedResult(httpContext);
    }
}
