using SixLabors.ImageSharp;

namespace OnlineConsulting.Storage.Common;

/// <summary>Shared by every provider - inspects an image's header only (Image.IdentifyAsync doesn't decode pixel data), so every backend reports the same Width/Height metadata without duplicating this logic three times.</summary>
public static class ImageDimensionReader
{
    public static async Task<(int? Width, int? Height)> TryReadAsync(Stream seekableStream, string contentType, CancellationToken cancellationToken)
    {
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return (null, null);

        var startPosition = seekableStream.Position;
        try
        {
            var info = await Image.IdentifyAsync(seekableStream, cancellationToken);
            return info is null ? (null, null) : (info.Width, info.Height);
        }
        catch (UnknownImageFormatException)
        {
            // Content-Type claimed "image/*" but the bytes aren't a format ImageSharp recognizes -
            // still a valid upload, just without dimension metadata.
            return (null, null);
        }
        finally
        {
            seekableStream.Position = startPosition;
        }
    }
}
