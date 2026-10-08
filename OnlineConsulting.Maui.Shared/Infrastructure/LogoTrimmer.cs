using Microsoft.JSInterop;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Crops a logo's empty (transparent or white) margins and caps its size, in the browser, so it can be shown at the height of the business name.
/// Streams both ways, so large files don't hit the interop message limit.</summary>
public sealed class LogoTrimmer(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private const long MaxTrimmedBytes = 5 * 1024 * 1024;

    private IJSObjectReference? _module;

    /// <summary>The cropped image as PNG, or null when it can't be read (e.g. SVG) or is blank; upload the original then.</summary>
    public async Task<byte[]?> TrimAsync(byte[] image, CancellationToken cancellationToken = default)
    {
        _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, "./_content/OnlineConsulting.Maui.Shared/logoTrim.js");

        using var input = new DotNetStreamReference(new MemoryStream(image));
        var trimmed = await _module.InvokeAsync<IJSStreamReference?>("trimLogo", cancellationToken, input);
        if (trimmed is null)
        {
            return null;
        }

        await using (trimmed)
        {
            await using var stream = await trimmed.OpenReadStreamAsync(MaxTrimmedBytes, cancellationToken);
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken);
            return buffer.ToArray();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }
    }
}
