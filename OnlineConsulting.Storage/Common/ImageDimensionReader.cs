using SkiaSharp;

namespace OnlineConsulting.Storage.Common;

/// <summary>Shared by every provider - inspects an image's header only (SKBitmap.DecodeBounds doesn't decode pixel data), so every backend reports the same Width/Height metadata without duplicating this logic three times.</summary>
public static class ImageDimensionReader
{
    /// <summary>Reads width/height from an image header; returns (null, null) for a non-image content type or bytes SkiaSharp can't recognize, rather than throwing.</summary>
    public static Task<(int? Width, int? Height)> TryReadAsync(Stream seekableStream, string contentType, CancellationToken cancellationToken)
    {
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<(int?, int?)>((null, null));
        }

        var startPosition = seekableStream.Position;

        try
        {
            var info = SKBitmap.DecodeBounds(seekableStream);

            return Task.FromResult<(int?, int?)>(info.Width > 0 && info.Height > 0 ? (info.Width, info.Height) : (null, null));
        }
        finally
        {
            seekableStream.Position = startPosition;
        }
    }
}
