using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using OnlineConsulting.Modules.Media.Application.Features.UploadMediaAsset;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class UploadMediaAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/media", Handle)
            .WithTags("Media")
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UploadMediaAsset")
            .WithDescription("Uploads a file (image) and registers it as a MediaAsset - the returned id can be referenced from any module that needs to show an image.");
    }

    private static async Task<IResult> Handle(IFormFile file, string? altText, ISender sender, HttpContext httpContext)
    {
        if (file.Length == 0)
            return Results.BadRequest(MediaMessages.NoFileProvided);

        await using var stream = file.OpenReadStream();
        var command = new UploadMediaAssetCommand(stream, file.FileName, file.ContentType, altText);
        var result = await sender.Send(command);

        return result.ToEnvelopedResult(httpContext);
    }
}
