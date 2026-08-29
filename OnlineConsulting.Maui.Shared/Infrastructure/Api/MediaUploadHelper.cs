using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Uploads a picked file via POST /api/media, then fetches its Url for immediate display - used by every dialog that lets an admin attach a cover/gallery image.</summary>
public static class MediaUploadHelper
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public static async Task<ApiEnvelope<MediaAssetResponse>> UploadAsync(IApiClient apiClient, IBrowserFile file, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream(MaxFileSizeBytes, cancellationToken);
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var uploadResult = await apiClient.PostFileAsync<Guid>(ApiRoutes.Media.Upload, content, cancellationToken);
        if (!uploadResult.IsSuccessful)
        {
            return new ApiEnvelope<MediaAssetResponse>(null, false, uploadResult.StatusCode, uploadResult.StatusMessage, uploadResult.Errors);
        }

        return await apiClient.GetAsync<MediaAssetResponse>($"/api/media/{uploadResult.ResultData}", cancellationToken);
    }

    /// <summary>Resolves a MediaAsset id to its display Url - null if the asset is missing or the request fails.</summary>
    public static async Task<string?> GetUrlAsync(IApiClient apiClient, Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<MediaAssetResponse>($"/api/media/{mediaAssetId}", cancellationToken);
        return result.IsSuccessful ? result.ResultData?.Url : null;
    }
}
