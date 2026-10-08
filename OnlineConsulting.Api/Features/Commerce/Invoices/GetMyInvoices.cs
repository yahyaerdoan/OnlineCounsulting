using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetMyInvoices;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetMyInvoices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/mine", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetMyInvoices")
            .WithDescription("The current user's invoices and receipts, newest first.")
            .ProducesEnveloped<List<InvoiceResponse>>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMyInvoicesQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
