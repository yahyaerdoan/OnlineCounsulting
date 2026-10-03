using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.PayInvoice;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class PayMyInvoice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/invoices/{id:guid}/pay", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("PayMyInvoice")
            .WithDescription("Starts the card payment for one of the current user's open invoices; returns a client secret to confirm, or Paid when it settled at once.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new PayInvoiceCommand(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
