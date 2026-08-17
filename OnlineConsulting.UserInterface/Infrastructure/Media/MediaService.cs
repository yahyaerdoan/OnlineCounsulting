using OnlineConsulting.UserInterface.Infrastructure.Api;
using System.Net.Http.Headers;

namespace OnlineConsulting.UserInterface.Infrastructure.Media;

public class MediaService(IApiClient apiClient) : IMediaService
{
    private const string MediaPath = "/api/media";

    public async Task<Guid?> UploadAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file is not { Length: > 0 })
            return null;

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        // POST /api/media returns OperationDataResult<Guid> - just the new asset's id, not the full
        // MediaAssetResponse shape (that only comes back from GET, used by ResolveUrlAsync below).
        var result = await apiClient.PostFileAsync<Guid>(MediaPath, content, cancellationToken);
        return result.IsSuccessful ? result.ResultData : null;
    }

    public async Task<string?> ResolveUrlAsync(Guid? mediaAssetId, CancellationToken cancellationToken = default)
    {
        if (mediaAssetId is null)
            return null;

        var result = await apiClient.GetAsync<MediaAssetResponse>($"{MediaPath}/{mediaAssetId}", cancellationToken);
        return result.ResultData?.Url;
    }

    private record MediaAssetResponse(Guid Id, string Url);
}
