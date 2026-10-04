using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoicePdf;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetMyInvoicePdf : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/{id:guid}/pdf", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetMyInvoicePdf")
            .WithDescription("One of the current user's invoices as a PDF (base64 in the envelope).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetInvoicePdfQuery(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
