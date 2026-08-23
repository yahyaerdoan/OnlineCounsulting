namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Shared shape of ApiEnvelope/ApiEnvelope&lt;T&gt; so form-error handling works against either.</summary>
public interface IApiResult
{
    string DisplayMessage { get; }
    Dictionary<string, List<string>>? FieldErrors { get; }
}

/// <summary>Mirrors the ResponseResultHandler envelope every Api endpoint already returns (resultData/isSuccessful/statusCode/statusMessage/errors) - deserialized case-insensitively since the Api serializes camelCase.</summary>
public record ApiEnvelope(bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors = null) : IApiResult
{
    /// <summary>Errors is more actionable than StatusMessage when present ("prefer Detail over Title").</summary>
    public string DisplayMessage => Errors is { Count: > 0 } ? string.Join(" ", Errors) : StatusMessage ?? "An error occurred.";
}

public record ApiEnvelope<T>(T? ResultData, bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors = null) : IApiResult
{
    public string DisplayMessage => Errors is { Count: > 0 } ? string.Join(" ", Errors) : StatusMessage ?? "An error occurred.";
}
