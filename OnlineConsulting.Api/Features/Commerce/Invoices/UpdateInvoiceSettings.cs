using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.UpdateInvoiceSettings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public class UpdateInvoiceSettings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/invoices/settings", Handle)
            .WithTags("Commerce/Invoices")
            .RequireAuthorization()
            .WithName("UpdateInvoiceSettings")
            .WithDescription("Sets the caller's business's invoicing preferences: days until a service-visit invoice is due (0 to 365). Tenant admins only.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] UpdateInvoiceSettingsRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateInvoiceSettingsRequest(int PaymentTermsDays)
{
    public UpdateInvoiceSettingsCommand ToCommand() => new(PaymentTermsDays);
}
