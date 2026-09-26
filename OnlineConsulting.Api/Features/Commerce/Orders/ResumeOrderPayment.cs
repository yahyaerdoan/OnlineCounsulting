using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class ResumeOrderPayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders/{id:guid}/resume-payment", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("ResumeOrderPayment")
            .WithDescription("Re-fetches a fresh PaymentClientSecret for the current user's own still-unpaid order.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new ResumeOrderPaymentQuery(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
