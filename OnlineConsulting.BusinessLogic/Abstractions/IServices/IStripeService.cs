using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IStripeService
{
    Task<IOperationResult<string>> CreatePaymentIntentAsync(decimal amount, string? description = null, string currency = "usd");
}
