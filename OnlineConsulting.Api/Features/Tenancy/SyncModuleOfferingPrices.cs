using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.SyncModuleOfferingPrices;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class SyncModuleOfferingPrices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/admin/module-offerings/sync-prices", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("SyncModuleOfferingPrices")
            .WithDescription("Re-creates provider prices missing on the active payment provider (SuperAdmin).")
            .ProducesEnveloped<int>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new SyncModuleOfferingPricesCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
