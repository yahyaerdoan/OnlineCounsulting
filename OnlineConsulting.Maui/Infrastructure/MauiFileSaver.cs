using OnlineConsulting.Maui.Shared.Infrastructure.Files;

namespace OnlineConsulting.Maui.Infrastructure;

/// <summary>A BlazorWebView can't do browser downloads, so the file is written to the app cache and opened in the device's viewer
/// (which offers save and share from there).</summary>
public sealed class MauiFileSaver : IFileSaver
{
    public async Task SaveAsync(string fileName, byte[] content, string contentType)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, Path.GetFileName(fileName));
        await File.WriteAllBytesAsync(path, content);
        await MainThread.InvokeOnMainThreadAsync(() => Launcher.OpenAsync(new OpenFileRequest(fileName, new ReadOnlyFile(path, contentType))));
    }
}
