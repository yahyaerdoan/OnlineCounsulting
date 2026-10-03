using Microsoft.JSInterop;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Files;

/// <summary>Hands a generated file (an invoice PDF) to the user: a browser download on the web, the device's viewer in the app.</summary>
public interface IFileSaver
{
    Task SaveAsync(string fileName, byte[] content, string contentType);
}

public sealed class JsFileSaver(IJSRuntime jsRuntime) : IFileSaver
{
    public async Task SaveAsync(string fileName, byte[] content, string contentType) =>
        await jsRuntime.InvokeVoidAsync("comfortProSaveFile", fileName, Convert.ToBase64String(content), contentType);
}
