using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.UpdateService;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Catalog;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class UpdateService : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/services/{id:guid}", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("UpdateService")
            .WithDescription("Updates an existing service.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateServiceRequest(Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment, bool IsEmergencyAvailable = false, Guid? CoverMediaAssetId = null, string PriceType = ServicePriceTypes.Fixed, decimal? PriceMax = null, string Kind = ServiceKinds.Booking)
{
    public UpdateServiceCommand ToCommand(Guid id) => new(id, CategoryId, Title, Description, DetailedDescription, Price, FeaturedArea, DiscountRate, TaxRate, RequiresPrepayment, IsEmergencyAvailable, CoverMediaAssetId, PriceType, PriceMax, Kind);
}
