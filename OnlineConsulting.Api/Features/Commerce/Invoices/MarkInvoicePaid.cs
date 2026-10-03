using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.MarkInvoicePaid;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public sealed record MarkInvoicePaidRequest(string? PaymentMethod);

public class MarkInvoicePaid : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/invoices/admin/{id:guid}/mark-paid", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("MarkInvoicePaid")
            .WithDescription("Records an offline payment (Cash, Check or Card taken on site) and emails the customer a receipt (staff).");
    }

    private static async Task<IResult> Handle(Guid id, MarkInvoicePaidRequest? body, ISender sender, HttpContext httpContext)
        => (await sender.Send(new MarkInvoicePaidCommand(id, body?.PaymentMethod ?? "Cash"))).ToEnvelopedResult(httpContext);
}
