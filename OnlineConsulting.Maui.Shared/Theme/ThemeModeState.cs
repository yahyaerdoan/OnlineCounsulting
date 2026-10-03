using Microsoft.JSInterop;

namespace OnlineConsulting.Maui.Shared.Theme;

public enum ThemeMode
{
    Light,
    System,
    Dark,
}

/// <summary>The user's light/system/dark choice, shared by the admin and marketing layouts so switching layouts keeps the same look; persisted in localStorage.</summary>
public sealed class ThemeModeState(IJSRuntime jsRuntime)
{
    private const string StorageKey = "comfortpro-theme-mode";
    private const string LegacyDarkModeKey = "comfortpro-dark-mode";

    private bool _loaded;

    public ThemeMode Mode { get; private set; } = ThemeMode.System;

    public event Func<Task>? Changed;

    /// <summary>Reads the saved choice once per app lifetime; migrates the old boolean dark-mode key.</summary>
    public async Task LoadAsync()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        var stored = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var mode))
        {
            Mode = mode;
            return;
        }

        var legacy = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", LegacyDarkModeKey);
        if (bool.TryParse(legacy, out var legacyDark))
        {
            Mode = legacyDark ? ThemeMode.Dark : ThemeMode.Light;
        }
    }

    public async Task SetAsync(ThemeMode mode)
    {
        Mode = mode;
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, mode.ToString());
        if (Changed is not null)
        {
            await Changed.Invoke();
        }
    }
}
