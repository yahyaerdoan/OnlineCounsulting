using Microsoft.Extensions.Logging;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Runs an IPaymentGateway or ISubscriptionGateway call, converting any exception into a 502 Bad Gateway OperationResult so callers don't repeat try/catch.
/// This is the only place the original exception survives - pass a logger or the real cause (declined card, misconfigured Connect account, etc.) is lost.</summary>
public static class PaymentGatewayCall
{
    /// <summary>Null on success, or a Bad Gateway result the caller should return immediately.</summary>
    public static async Task<OperationResult?> RunAsync(Func<Task> action, string failureMessage, ILogger? logger = null)
    {
        try
        {
            await action();
            return null;
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Payment gateway call failed: {FailureMessage}", failureMessage);
            return Result.BadGateway(failureMessage);
        }
    }

    /// <summary>Like RunAsync, for a call that returns a value - own name, not an overload, since a value-returning action would also match RunAsync and confuse the compiler.</summary>
    public static async Task<(OperationResult? Failure, T? Value)> RunWithResultAsync<T>(Func<Task<T>> action, string failureMessage, ILogger? logger = null)
    {
        try
        {
            return (null, await action());
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Payment gateway call failed: {FailureMessage}", failureMessage);
            return (Result.BadGateway(failureMessage), default);
        }
    }
}
