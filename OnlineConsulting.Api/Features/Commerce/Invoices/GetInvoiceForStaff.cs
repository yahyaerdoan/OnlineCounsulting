using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceForStaff;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetInvoiceForStaff : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/admin/{id:guid}", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetInvoiceForStaff")
            .WithDescription("Any invoice for the tenant with its lines (staff).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetInvoiceForStaffQuery(id))).ToEnvelopedResult(httpContext);
}
