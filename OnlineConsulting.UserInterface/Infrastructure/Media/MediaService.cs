using OnlineConsulting.UserInterface.Infrastructure.Api;
using System.Net.Http.Headers;

namespace OnlineConsulting.UserInterface.Infrastructure.Media;

public class MediaService(IApiClient apiClient) : IMediaService
{
    private const string MediaPath = "/api/media";

    /// <summary>Uploads the file and returns the new asset's id (POST /api/media returns only the id, not the
    /// full asset shape - that comes back from GET via <see cref="ResolveUrlAsync"/>).</summary>
    public async Task<Guid?> UploadAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file is not { Length: > 0 })
        {
            return null;
        }

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        var result = await apiClient.PostFileAsync<Guid>(MediaPath, content, cancellationToken);
        return result.IsSuccessful ? result.ResultData : null;
    }

    public async Task<string?> ResolveUrlAsync(Guid? mediaAssetId, CancellationToken cancellationToken = default)
    {
        if (mediaAssetId is null)
        {
            return null;
        }

        var result = await apiClient.GetAsync<MediaAssetResponse>($"{MediaPath}/{mediaAssetId}", cancellationToken);
        return result.ResultData?.Url;
    }

    private record MediaAssetResponse(Guid Id, string Url);
}
