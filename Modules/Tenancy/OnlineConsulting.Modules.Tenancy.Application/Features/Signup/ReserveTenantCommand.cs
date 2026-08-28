using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>First half of self-service tenant signup - public, no auth. Creates/reuses the Tenant and its PendingPayment items, no billing yet. Split out so signup can run the free email-uniqueness check before the irreversible Stripe charge.</summary>
public record ReserveTenantCommand(
    string CompanyName,
    List<string> ModuleKeys,
    string AdminEmail) : IRequest<OperationDataResult<ReserveTenantResult>>, ITransactionAddRequest;

public record ReserveTenantResult(Guid TenantId);

public class ReserveTenantHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository,
    IModuleOfferingRepository moduleOfferingRepository,
    ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<ReserveTenantCommand, OperationDataResult<ReserveTenantResult>>
{
    public async Task<OperationDataResult<ReserveTenantResult>> Handle(ReserveTenantCommand request, CancellationToken cancellationToken)
    {
        var requestedKeys = request.ModuleKeys.Distinct().ToList();

        if (requestedKeys.Count > 1 && !subscriptionGateway.SupportsMultipleItems)
        {
            return Result.BadRequest<ReserveTenantResult>(SignupMessages.MultipleModulesNotSupportedByProvider);
        }

        var offerings = await moduleOfferingRepository.GetListAsync(
            predicate: m => requestedKeys.Contains(m.Key) && m.IsPubliclyVisible,
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        var offeringsByKey = offerings.Items.ToDictionary(m => m.Key);

        var missingKeys = requestedKeys.Where(k => !offeringsByKey.ContainsKey(k)).ToList();
        if (missingKeys.Count > 0)
        {
            return Result.BadRequest<ReserveTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, string.Join(", ", missingKeys)));
        }

        var selectedOfferings = requestedKeys.Select(k => offeringsByKey[k]).ToList();
        var invalidOffering = selectedOfferings.FirstOrDefault(o => o.ProviderPriceId is null);
        if (invalidOffering is not null)
        {
            return Result.BadRequest<ReserveTenantResult>(string.Format(SignupMessages.UnknownOrUnavailableModuleKeysFormat, invalidOffering.Key));
        }

        var tenant = await tenantRepository.GetAsync(
            t => t.PrimaryContactEmail == request.AdminEmail &&
                 (t.Status == TenantStatuses.PendingPayment || t.Status == TenantStatuses.Failed),
            cancellationToken: cancellationToken);

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
                return Result.BadRequest<ReserveTenantResult>(SignupMessages.SlugAlreadyTaken);
            }

            tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.CompanyName,
                Slug = slug,
                Status = TenantStatuses.PendingPayment,
                PrimaryContactEmail = request.AdminEmail,
            };

            tenantSubscription = new TenantSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Status = TenantSubscriptionStatuses.PendingPayment,
                StartDate = DateTime.UtcNow,
            };

            _ = await tenantRepository.AddAsync(tenant);
            _ = await tenantSubscriptionRepository.AddAsync(tenantSubscription);
            existingItems = [];
        }
        else
        {
            tenantSubscription = await tenantSubscriptionRepository.GetAsync(
                s => s.TenantId == tenant.Id, cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException($"Tenant {tenant.Id} is pending/failed but has no TenantSubscription row.");

            var existingItemsPage = await tenantSubscriptionItemRepository.GetListAsync(
                predicate: i => i.TenantSubscriptionId == tenantSubscription.Id,
                size: RepositoryQuerySize.Unbounded,
                cancellationToken: cancellationToken);
            existingItems = existingItemsPage.Items.ToList();
        }

        var alreadyRecordedKeys = existingItems.Select(i => i.ModuleKey).ToHashSet();

        foreach (var offering in selectedOfferings.Where(o => !alreadyRecordedKeys.Contains(o.Key)))
        {
            var item = new TenantSubscriptionItem
            {
                Id = Guid.NewGuid(),
                TenantSubscriptionId = tenantSubscription.Id,
                ModuleKey = offering.Key,
                Status = TenantSubscriptionItemStatuses.Pending,
                ProviderSubscriptionItemId = null,
                PriceAtAddition = offering.Price,
                AddedAt = DateTime.UtcNow,
            };
            _ = await tenantSubscriptionItemRepository.AddAsync(item);
        }

        return Result.Created(new ReserveTenantResult(tenant.Id), "Tenant reserved successfully.");
    }
}
