using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Public, self-service tenant signup - no auth required (a Tenant doesn't exist yet for the caller to authenticate against). AdminFirstName/LastName/Email/Password ride along on the wire contract but this handler never touches Identity directly (no established precedent in this codebase for one module's Application layer depending on another module's domain/Identity types) - it only creates the Tenant/TenantSubscription/TenantSubscriptionItem rows and hands back TenantId; the Api endpoint (OnlineConsulting.Api/Features/Tenancy/SignUp.cs) issues a second, separate ISender.Send to Identity's CreateTenantAdminCommand right after this succeeds, mirroring how SubscribeToMembership.cs already chains multiple modules' commands from the Api layer. PaymentMethodId comes from the provider's client-side SDK (Stripe.js), already tokenized.</summary>
public record SignUpTenantCommand(
    string CompanyName,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    List<string> ModuleKeys,
    string PaymentMethodId) : IRequest<OperationDataResult<SignUpTenantResult>>, ITransactionAddRequest;

/// <summary>ClientSecret mirrors SubscribeToMembershipResult - null for Stripe, a PayPal approval URL when PayPal is active.</summary>
public record SignUpTenantResult(Guid TenantId, string? ClientSecret);

public class SignUpTenantHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository,
    IModuleOfferingRepository moduleOfferingRepository,
    ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<SignUpTenantCommand, OperationDataResult<SignUpTenantResult>>
{
    public async Task<OperationDataResult<SignUpTenantResult>> Handle(SignUpTenantCommand request, CancellationToken cancellationToken)
    {
        var slug = Slugify(request.CompanyName);

        var slugAlreadyTaken = await tenantRepository.AnyAsync(t => t.Slug == slug, cancellationToken: cancellationToken);
        if (slugAlreadyTaken)
            return Result.BadRequest<SignUpTenantResult>(SignupMessages.SlugAlreadyTaken);

        var requestedKeys = request.ModuleKeys.Distinct().ToList();

        var offerings = await moduleOfferingRepository.GetListAsync(
            predicate: m => requestedKeys.Contains(m.Key) && m.IsPubliclyVisible,
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        var offeringsByKey = offerings.Items.ToDictionary(m => m.Key);

        var missingKeys = requestedKeys.Where(k => !offeringsByKey.ContainsKey(k)).ToList();
        if (missingKeys.Count > 0)
            return Result.BadRequest<SignUpTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, string.Join(", ", missingKeys)));

        var selectedOfferings = requestedKeys.Select(k => offeringsByKey[k]).ToList();
        var invalidOffering = selectedOfferings.FirstOrDefault(o => o.ProviderPriceId is null);
        if (invalidOffering is not null)
            return Result.BadRequest<SignUpTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, invalidOffering.Key));

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName,
            Slug = slug,
            Status = TenantStatuses.PendingPayment,
            PrimaryContactEmail = request.AdminEmail,
        };

        var tenantSubscription = new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Status = TenantSubscriptionStatuses.PendingPayment,
            StartDate = DateTime.UtcNow,
        };

        var customer = await subscriptionGateway.EnsureCustomerAsync(new EnsureCustomerRequest(tenantSubscription.Id.ToString(), request.AdminEmail), cancellationToken);
        tenant.ProviderCustomerId = customer.ProviderCustomerId;

        var firstOffering = selectedOfferings[0];
        var firstOfferingPriceId = firstOffering.ProviderPriceId
            ?? throw new InvalidOperationException($"ModuleOffering {firstOffering.Key} has no ProviderPriceId.");

        var subscription = await subscriptionGateway.CreateSubscriptionAsync(
            new CreateSubscriptionRequest(customer.ProviderCustomerId, firstOfferingPriceId, request.PaymentMethodId, tenantSubscription.Id.ToString()),
            cancellationToken);

        tenantSubscription.ProviderSubscriptionId = subscription.ProviderSubscriptionId;
        tenantSubscription.RenewalDate = subscription.CurrentPeriodEnd.UtcDateTime;
        tenantSubscription.Status = subscription.Status switch
        {
            PaymentStatuses.Succeeded => TenantSubscriptionStatuses.Active,
            PaymentStatuses.Failed => TenantSubscriptionStatuses.PastDue,
            _ => TenantSubscriptionStatuses.PendingPayment,
        };

        tenant.Status = tenantSubscription.Status switch
        {
            TenantSubscriptionStatuses.Active => TenantStatuses.Active,
            TenantSubscriptionStatuses.PastDue => TenantStatuses.PastDue,
            _ => TenantStatuses.PendingPayment,
        };

        var now = DateTime.UtcNow;
        var items = new List<TenantSubscriptionItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TenantSubscriptionId = tenantSubscription.Id,
                ModuleKey = firstOffering.Key,
                ProviderSubscriptionItemId = subscription.FirstItemProviderId,
                PriceAtAddition = firstOffering.Price,
                AddedAt = now,
            },
        };

        foreach (var offering in selectedOfferings.Skip(1))
        {
            var offeringPriceId = offering.ProviderPriceId
                ?? throw new InvalidOperationException($"ModuleOffering {offering.Key} has no ProviderPriceId.");

            var providerSubscriptionItemId = await subscriptionGateway.AddSubscriptionItemAsync(
                tenantSubscription.ProviderSubscriptionId, offeringPriceId, cancellationToken);

            items.Add(new TenantSubscriptionItem
            {
                Id = Guid.NewGuid(),
                TenantSubscriptionId = tenantSubscription.Id,
                ModuleKey = offering.Key,
                ProviderSubscriptionItemId = providerSubscriptionItemId,
                PriceAtAddition = offering.Price,
                AddedAt = now,
            });
        }

        await tenantRepository.AddAsync(tenant);
        await tenantSubscriptionRepository.AddAsync(tenantSubscription);
        foreach (var item in items)
            await tenantSubscriptionItemRepository.AddAsync(item);

        return Result.Created(new SignUpTenantResult(tenant.Id, subscription.ClientSecret), "Tenant signed up successfully.");
    }

    private static string Slugify(string companyName)
    {
        var lowered = companyName.Trim().ToLowerInvariant();
        var hyphenated = System.Text.RegularExpressions.Regex.Replace(lowered, @"[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrEmpty(hyphenated) ? Guid.NewGuid().ToString("N") : hyphenated;
    }
}
