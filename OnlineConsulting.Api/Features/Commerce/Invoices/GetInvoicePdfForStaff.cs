using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoicePdfForStaff;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetInvoicePdfForStaff : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/admin/{id:guid}/pdf", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetInvoicePdfForStaff")
            .WithDescription("Any invoice for the tenant as a PDF (staff).")
            .ProducesEnveloped<InvoicePdfResponse>();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetInvoicePdfForStaffQuery(id))).ToEnvelopedResult(httpContext);
}
