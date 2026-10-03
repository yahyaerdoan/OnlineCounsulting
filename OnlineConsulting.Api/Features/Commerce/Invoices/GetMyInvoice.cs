using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceById;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetMyInvoice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/{id:guid}", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetMyInvoice")
            .WithDescription("One of the current user's invoices with its lines.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetInvoiceByIdQuery(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
