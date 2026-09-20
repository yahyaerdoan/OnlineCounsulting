using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Runs an IPaymentGateway or ISubscriptionGateway call, converting any exception into a BadRequest OperationResult so callers don't repeat try/catch.</summary>
public static class PaymentGatewayCall
{
    /// <summary>Null on success, or a BadRequest result the caller should return immediately.</summary>
    public static async Task<OperationResult?> RunAsync(Func<Task> action, string failureMessage)
    {
        try
        {
            await action();
            return null;
        }
        catch (Exception)
        {
            return Result.BadRequest(failureMessage);
        }
    }

    /// <summary>Like RunAsync, for a call that returns a value - own name, not an overload, since a value-returning action would also match RunAsync and confuse the compiler.</summary>
    public static async Task<(OperationResult? Failure, T? Value)> RunWithResultAsync<T>(Func<Task<T>> action, string failureMessage)
    {
        try
        {
            return (null, await action());
        }
        catch (Exception)
        {
            return (Result.BadRequest(failureMessage), default);
        }
    }
}
