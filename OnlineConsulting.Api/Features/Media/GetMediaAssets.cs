using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.GetMediaAssets;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class GetMediaAssets : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/media", Handle)
            .WithTags("Media")
            .RequireAuthorization()
            .WithName("GetMediaAssets")
            .WithDescription("Returns the tenant's uploaded media assets, paginated - powers an admin media library picker.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetMediaAssetsQuery(PageRequestFactory.Create(index, size)));
        if (result.IsSuccessful)
        {
            for (var i = 0; i < result.Data.Items.Count; i++)
            {
                result.Data.Items[i] = result.Data.Items[i] with { Url = MediaUrlResolver.Resolve(result.Data.Items[i].Url, httpContext) };
            }
        }

        return result.ToEnvelopedResult(httpContext);
    }
}
