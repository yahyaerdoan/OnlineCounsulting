namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Mirrors the ResponseResultHandler envelope every Api endpoint already returns (resultData/isSuccessful/statusCode/statusMessage/errors) - deserialized case-insensitively since the Api serializes camelCase.</summary>
public record ApiEnvelope(bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors)
{
    /// <summary>Errors is more actionable than StatusMessage when present ("prefer Detail over Title").</summary>
    public string DisplayMessage => Errors is { Count: > 0 } ? string.Join(" ", Errors) : StatusMessage ?? "An error occurred.";
}

public record ApiEnvelope<T>(T? ResultData, bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors)
{
    public string DisplayMessage => Errors is { Count: > 0 } ? string.Join(" ", Errors) : StatusMessage ?? "An error occurred.";
}
