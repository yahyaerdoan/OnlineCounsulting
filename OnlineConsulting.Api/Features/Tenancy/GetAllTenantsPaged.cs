using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetAllTenantsPaged;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetAllTenantsPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/tenants/query", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetAllTenantsPaged")
            .WithDescription("Returns every tenant on the platform, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with active module and pricing summary (SuperAdmin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllTenantsPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
