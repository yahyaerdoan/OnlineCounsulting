using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class ResumeOrderPayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/orders/{id:guid}/resume-payment", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("ResumeOrderPayment")
            .WithDescription("Re-fetches a fresh PaymentClientSecret for the current user's own still-unpaid order.")
            .ProducesEnveloped<CreateOrderResult>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new ResumeOrderPaymentQuery(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
