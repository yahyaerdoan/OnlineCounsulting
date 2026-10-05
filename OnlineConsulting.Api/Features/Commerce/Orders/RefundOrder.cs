using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.RefundOrder;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class RefundOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/orders/{id:guid}/refund", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("RefundOrder")
            .WithDescription("Refunds a paid order through whichever payment provider processed it. Amount omitted means a full refund.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] RefundOrderRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record RefundOrderRequest(decimal? Amount = null)
{
    public RefundOrderCommand ToCommand(Guid orderId) => new(orderId, Amount);
}
