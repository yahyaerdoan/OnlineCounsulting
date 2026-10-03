using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.SyncInvoicePayment;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class SyncMyInvoicePayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/invoices/{id:guid}/sync-payment", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("SyncMyInvoicePayment")
            .WithDescription("Checks the provider for the current user's open invoice payment and settles it at once if the charge already succeeded (no wait for the webhook).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new SyncInvoicePaymentCommand(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
