using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceSettings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class GetInvoiceSettings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/invoices/settings", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("GetInvoiceSettings")
            .WithDescription("Returns the caller's business's invoicing preferences (payment terms). Tenant admins only.")
            .ProducesEnveloped<InvoiceSettingsResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetInvoiceSettingsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
