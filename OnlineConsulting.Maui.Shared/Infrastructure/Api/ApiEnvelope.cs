namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Shared shape of ApiEnvelope and its generic variant, so form-error handling works against either.</summary>
public interface IApiResult
{
    string DisplayMessage { get; }
    Dictionary<string, List<string>>? FieldErrors { get; }
}

/// <summary>Mirrors the Api's ResponseResultHandler envelope.</summary>
public record ApiEnvelope(bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors = null) : IApiResult
{
    public string DisplayMessage => ApiMessages.Display(StatusCode, StatusMessage, Errors);
}

public record ApiEnvelope<T>(T? ResultData, bool IsSuccessful, int StatusCode, string? StatusMessage, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors = null) : IApiResult
{
    public string DisplayMessage => ApiMessages.Display(StatusCode, StatusMessage, Errors);
}
