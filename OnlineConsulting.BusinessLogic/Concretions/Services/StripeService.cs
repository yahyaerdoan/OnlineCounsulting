using MediatR;
using Microsoft.Extensions.Options;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using Stripe;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class StripeService(ISender sender, IOptions<AppSettingStripeOption> stripeOptions) : IStripeService
{
    public async Task<IOperationResult<string>> CreatePaymentIntentAsync(decimal amount, string? description = null, string currency = "usd")
    {
        var result = await sender.Send(new GetCurrentUserQuery());
        if (!result.IsSuccessful || result.Data is null)
            return new ErrorDataResult<string>("User not found or not logged in.", ResultStatus.Unauthorized);

        var userData = result.Data;

        StripeConfiguration.ApiKey = stripeOptions.Value.SecretKey;

        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Name = userData.FirstName + " " + userData.LastName,
            Email = userData.Email,
        });

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency,
            Description = description ?? "Online Consulting Payment",
            PaymentMethodTypes = ["card",],
            Customer = customer.Id,
        });

        return new SuccessDataResult<string>(paymentIntent.ClientSecret, "Payment intent created successfully.", ResultStatus.Created);
    }
}
