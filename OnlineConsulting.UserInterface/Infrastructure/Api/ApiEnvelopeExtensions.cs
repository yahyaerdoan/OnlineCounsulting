using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Mapping;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>ApiEnvelope&lt;T&gt; is a separate record rather than a subclass of ApiEnvelope, so admin feature
/// services that only need the success/message half of a create-style call drop the payload through this.</summary>
public static class ApiEnvelopeExtensions
{
    public static ApiEnvelope WithoutData<T>(this ApiEnvelope<T> envelope) =>
        new(envelope.IsSuccessful, envelope.StatusCode, envelope.StatusMessage, envelope.Errors);

    /// <summary>Lets callers that pre-date IApiClient (built against ResultHandler's OperationResult directly,
    /// e.g. AccountController) keep working unchanged against an Api-backed service.</summary>
    public static OperationResult ToOperationResult(this ApiEnvelope envelope) =>
        envelope.IsSuccessful
            ? Result.Success(envelope.StatusMessage ?? "Success.")
            : Result.Failure(envelope.StatusMessage ?? "Error.", envelope.DisplayMessage, envelope.StatusCode.ToResultStatus());
}
