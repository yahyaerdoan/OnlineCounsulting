using Hateoas.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Constants;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.UploadMediaAsset;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class UploadMediaAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/media", Handle)
            .WithTags("Media")
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UploadMediaAsset")
            .WithCreatedLocation("GetMediaAsset")
            .WithDescription("Uploads a file (image) and registers it as a MediaAsset - the returned id can be referenced from any module that needs to show an image.");
    }

    private static async Task<IResult> Handle(IFormFile file, [FromForm] string folder, [FromForm] string? altText, ISender sender, HttpContext httpContext)
    {
        if (file.Length == 0)
        {
            return Results.BadRequest(MediaMessages.NoFileProvided);
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadMediaAssetCommand(stream, file.FileName, file.ContentType, folder, altText);
        var result = await sender.Send(command);

        return result.ToEnvelopedResult(httpContext);
    }
}
