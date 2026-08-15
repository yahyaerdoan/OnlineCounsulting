using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Payments.Gateways;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Payments;

public static class PaymentsServiceCollectionExtensions
{
    /// <summary>Registers every gateway under its own keyed-DI slot (so a refund can explicitly target the provider that originally processed it) plus a plain, unkeyed IPaymentGateway resolved from Payment:ActiveProvider for ordinary checkout code that doesn't care which provider is active.</summary>
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymentOptions>(configuration.GetSection("Payment"));
        var activeProvider = configuration.GetSection("Payment")["ActiveProvider"]
            ?? throw new InvalidOperationException("Payment:ActiveProvider is not configured.");

        services.AddKeyedSingleton<IPaymentGateway, MockPaymentGateway>(PaymentProviderNames.Mock);
        services.AddKeyedSingleton<IPaymentGateway, StripePaymentGateway>(PaymentProviderNames.Stripe);

        services.AddHttpClient<PayPalPaymentGateway>((serviceProvider, client) =>
        {
            var payPalOptions = configuration.GetSection("Payment:PayPal").Get<PayPalOptions>() ?? new PayPalOptions();
            client.BaseAddress = new Uri(payPalOptions.UseSandbox ? "https://api-m.sandbox.paypal.com" : "https://api-m.paypal.com");
        });
        services.AddKeyedSingleton<IPaymentGateway>(PaymentProviderNames.PayPal, (serviceProvider, _) => serviceProvider.GetRequiredService<PayPalPaymentGateway>());

        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredKeyedService<IPaymentGateway>(activeProvider));

        return services;
    }
}
