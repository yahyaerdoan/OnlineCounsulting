using Hateoas.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.CreateService;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Catalog;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class CreateService : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/services", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("CreateService")
            .WithCreatedLocation("GetServiceById")
            .WithDescription("Creates a new service in the catalog.");
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateServiceRequest(Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment = false, bool IsEmergencyAvailable = false, Guid? CoverMediaAssetId = null, string PriceType = ServicePriceTypes.Fixed, decimal? PriceMax = null, string Kind = ServiceKinds.Booking)
{
    public CreateServiceCommand ToCommand() => new(CategoryId, Title, Description, DetailedDescription, Price, FeaturedArea, DiscountRate, TaxRate, RequiresPrepayment, IsEmergencyAvailable, CoverMediaAssetId, PriceType, PriceMax, Kind);
}
