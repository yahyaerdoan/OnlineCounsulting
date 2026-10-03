using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetAllInvoicesPaged;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetAllInvoicesPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/invoices/admin/query", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetAllInvoicesPaged")
            .WithDescription("Every invoice for the tenant (staff), paginated, optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
        => (await sender.Send(new GetAllInvoicesPagedQuery(query.ToPageRequest(), dynamicQuery))).ToEnvelopedResult(httpContext);
}
