using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.VoidInvoice;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public sealed record VoidInvoiceRequest(string? Reason);

public class VoidInvoice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/invoices/admin/{id:guid}/void", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("VoidInvoice")
            .WithDescription("Voids an open invoice (staff).");
    }

    private static async Task<IResult> Handle(Guid id, VoidInvoiceRequest? body, ISender sender, HttpContext httpContext)
        => (await sender.Send(new VoidInvoiceCommand(id, body?.Reason))).ToEnvelopedResult(httpContext);
}
