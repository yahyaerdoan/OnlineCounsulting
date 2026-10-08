using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.SyncInvoicePayment;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class SyncMyInvoicePayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/invoices/{id:guid}/sync-payment", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("SyncMyInvoicePayment")
            .WithDescription("Checks the provider for the current user's open invoice payment and settles it at once if the charge already succeeded (no wait for the webhook).")
            .ProducesEnveloped<SyncInvoicePaymentResult>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new SyncInvoicePaymentCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
