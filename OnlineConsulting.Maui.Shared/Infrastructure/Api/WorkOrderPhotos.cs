namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

public sealed record WorkOrderPhotoUrls(string? Before, string? After);

/// <summary>Resolves a work order's first before and after photo to display Urls, shared by the admin detail page and the customer's equipment history.</summary>
public static class WorkOrderPhotos
{
    public static async Task<WorkOrderPhotoUrls> LoadAsync(IApiClient apiClient, WorkOrderResponse workOrder, CancellationToken cancellationToken = default)
    {
        var beforeItem = workOrder.MediaItems.OrderBy(m => m.DisplayOrder).FirstOrDefault(m => m.IsBeforePhoto);
        var afterItem = workOrder.MediaItems.OrderBy(m => m.DisplayOrder).FirstOrDefault(m => !m.IsBeforePhoto);

        var before = beforeItem is null ? null : await MediaUploadHelper.GetUrlAsync(apiClient, beforeItem.MediaAssetId, cancellationToken);
        var after = afterItem is null ? null : await MediaUploadHelper.GetUrlAsync(apiClient, afterItem.MediaAssetId, cancellationToken);

        return new WorkOrderPhotoUrls(before, after);
    }
}
