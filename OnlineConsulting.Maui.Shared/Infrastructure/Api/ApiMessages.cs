namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Picks the message a user sees for an API result: the server's own errors or message first, then a status-specific fallback, never a bare HTTP reason phrase.</summary>
public static class ApiMessages
{
    public static string Display(int statusCode, string? statusMessage, IReadOnlyCollection<string>? errors)
    {
        if (errors is { Count: > 0 })
        {
            return string.Join(" ", errors);
        }

        return string.IsNullOrWhiteSpace(statusMessage) ? FallbackFor(statusCode) : statusMessage;
    }

    private static string FallbackFor(int statusCode) => statusCode switch
    {
        401 => "Please sign in to continue.",
        403 => "You don't have permission to do that.",
        404 => "We couldn't find what you were looking for.",
        409 => "This no longer matches the current state. Please refresh and try again.",
        410 => "This link has expired.",
        422 => "Please check the highlighted fields and try again.",
        429 => "Too many requests. Please wait a moment and try again.",
        502 or 503 or 504 => "A service we depend on isn't responding. Please try again shortly.",
        >= 500 => "Something went wrong on our side. Please try again.",
        _ => "Something went wrong. Please try again.",
    };
}
