using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.ReserveTenant;

/// <summary>First half of self-service signup, public/no-auth - creates/reuses the Tenant + PendingPayment items so the free email-uniqueness check runs before the irreversible Stripe charge.</summary>
public record ReserveTenantCommand(string CompanyName, List<string> ModuleKeys, string AdminEmail) : IRequest<OperationDataResult<ReserveTenantResult>>, ITransactionAddRequest;

public class ReserveTenantHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<ReserveTenantCommand, OperationDataResult<ReserveTenantResult>>
{
    public async Task<OperationDataResult<ReserveTenantResult>> Handle(ReserveTenantCommand request, CancellationToken cancellationToken)
    {
        var requestedKeys = request.ModuleKeys.Distinct().ToList();

        if (requestedKeys.Count > 1 && !subscriptionGateway.SupportsMultipleItems)
        {
            return Result.UnprocessableContent<ReserveTenantResult>(SignupMessages.MultipleModulesNotSupportedByProvider);
        }

        var offerings = await moduleOfferingRepository.GetAllAsync(predicate: m => requestedKeys.Contains(m.Key) && m.IsPubliclyVisible, cancellationToken: cancellationToken);

        var offeringsByKey = offerings.ToDictionary(m => m.Key);

        var missingKeys = requestedKeys.Where(k => !offeringsByKey.ContainsKey(k)).ToList();

        if (missingKeys.Count > 0)
        {
            return Result.UnprocessableContent<ReserveTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, string.Join(", ", missingKeys)));
        }

        var selectedOfferings = requestedKeys.Select(k => offeringsByKey[k]).ToList();
        var invalidOffering = selectedOfferings.FirstOrDefault(o => o.ProviderPriceId is null);

        if (invalidOffering is not null)
        {
            return Result.UnprocessableContent<ReserveTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, invalidOffering.Key));
        }

        var tenant = await tenantRepository
            .GetAsync(t => t.PrimaryContactEmail == request.AdminEmail && (t.Status == TenantStatuses.PendingPayment || t.Status == TenantStatuses.Failed), cancellationToken: cancellationToken);

        TenantSubscription tenantSubscription;
        List<TenantSubscriptionItem> existingItems;

        if (tenant is null)
        {
            var slug = SlugGenerator.Slugify(request.CompanyName);
            if (string.IsNullOrEmpty(slug))
            {
                slug = Guid.NewGuid().ToString("N");
            }

            var slugAlreadyTaken = await tenantRepository.AnyAsync(t => t.Slug == slug, cancellationToken: cancellationToken);

            if (slugAlreadyTaken)
            {
                return Result.Conflict<ReserveTenantResult>(SignupMessages.SlugAlreadyTaken);
            }

            tenant = Tenant.Reserve(request.CompanyName, slug, request.AdminEmail);
            tenantSubscription = TenantSubscription.Start(tenant.Id, DateTime.UtcNow);

            _ = await tenantRepository.AddAsync(tenant, cancellationToken: cancellationToken);

            _ = await tenantSubscriptionRepository.AddAsync(tenantSubscription, cancellationToken: cancellationToken);

            existingItems = [];
        }
        else
        {
            tenantSubscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException($"Tenant {tenant.Id} is pending/failed but has no TenantSubscription row.");

            var existingItemsPage = await tenantSubscriptionItemRepository
                .GetAllAsync(predicate: i => i.TenantSubscriptionId == tenantSubscription.Id, enableTracking: true, cancellationToken: cancellationToken);

            existingItems = [.. existingItemsPage];

            if (tenantSubscription.ProviderSubscriptionId is null)
            {
                foreach (var dropped in existingItems.Where(i => !requestedKeys.Contains(i.ModuleKey)).ToList())
                {
                    _ = await tenantSubscriptionItemRepository.DeleteAsync(dropped, cancellationToken: cancellationToken);
                    _ = existingItems.Remove(dropped);
                }

                tenantSubscription.RestartSignup();
                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);
            }
        }

        var alreadyRecordedKeys = existingItems.Select(i => i.ModuleKey).ToHashSet();

        foreach (var offering in selectedOfferings.Where(o => !alreadyRecordedKeys.Contains(o.Key)))
        {
            var item = TenantSubscriptionItem.Add(tenantSubscription.Id, offering.Key, offering.Price, DateTime.UtcNow);

            _ = await tenantSubscriptionItemRepository.AddAsync(item, cancellationToken: cancellationToken);
        }

        return Result.Created(new ReserveTenantResult(tenant.Id), "Tenant reserved successfully.");
    }
}
