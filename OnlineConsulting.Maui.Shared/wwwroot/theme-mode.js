// Loaded in <head>: paints the page background in the saved light/dark choice (same keys as ThemeModeState) and hides the
// app until AppThemeProvider has drawn the real palette, so opening the app never flashes light first.
window.comfortProTheme = (function () {
    var root = document.documentElement;
    var mode = null;
    var legacy = null;
    try {
        mode = localStorage.getItem('comfortpro-theme-mode');
        legacy = localStorage.getItem('comfortpro-dark-mode');
    } catch (e) {
        // Blocked storage: follow the OS; reveal() must still exist for AppThemeProvider.
    }
    var systemIsDark = !!window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    var isDark = mode ? mode === 'Dark' || (mode === 'System' && systemIsDark) : legacy ? legacy === 'true' : systemIsDark;

    // Phone status bar follows the app bar color (App.razor puts both colors on the tag).
    function setStatusBar(dark) {
        var meta = document.querySelector('meta[name="theme-color"]');
        if (meta) {
            meta.setAttribute('content', meta.getAttribute(dark ? 'data-dark' : 'data-light'));
        }
    }

    setStatusBar(isDark);

    if (root.hasAttribute('data-app-interactive')) {
        root.setAttribute('data-theme-boot', isDark ? 'dark' : 'light');
    }

    function show() {
        root.removeAttribute('data-theme-boot');
    }

    // Shows the app once MudThemeProvider's drawn palette matches the chosen look (it can land a frame after the request).
    function reveal(dark) {
        var frames = 0;
        (function check() {
            var red = /\d+/.exec(getComputedStyle(root).getPropertyValue('--mud-palette-background'));
            if ((red && parseInt(red[0], 10) < 128) === dark || ++frames > 30) {
                show();
                return;
            }
            requestAnimationFrame(check);
        })();
    }

    // Never leave the app hidden if Blazor fails to start.
    setTimeout(show, 5000);

    return { reveal: reveal, setStatusBar: setStatusBar };
})();
