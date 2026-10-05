using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceById;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetMyInvoice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/{id:guid}", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetMyInvoice")
            .WithDescription("One of the current user's invoices with its lines.")
            .ProducesEnveloped<InvoiceResponse>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetInvoiceByIdQuery(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
