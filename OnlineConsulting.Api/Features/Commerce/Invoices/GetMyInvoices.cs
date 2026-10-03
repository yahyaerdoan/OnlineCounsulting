using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetMyInvoices;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetMyInvoices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/mine", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetMyInvoices")
            .WithDescription("The current user's invoices and receipts, newest first.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyInvoicesQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
