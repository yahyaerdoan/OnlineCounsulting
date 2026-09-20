using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

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
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ResumeOrderPaymentQuery(id, currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
