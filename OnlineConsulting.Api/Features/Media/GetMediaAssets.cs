using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.GetMediaAssets;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class GetMediaAssets : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/media", Handle)
            .WithTags("Media")
            .RequireAuthorization()
            .WithName("GetMediaAssets")
            .WithDescription("Returns the tenant's uploaded media assets, paginated - powers an admin media library picker.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetMediaAssetsQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
