using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using System.Globalization;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Rules;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.RemoveModule;

/// <summary>Mirror of AddModuleCommand - stops renewing one module: no refund, it stays usable until the paid period ends, then EndingModuleService removes it. Same authorization: tenant admins (own tenant, via TenantOwnershipGuard) or a SuperAdmin.</summary>
public record RemoveModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class RemoveModuleHandler(ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionGateway subscriptionGateway, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor,
    ITenantTimeZoneReader timeZoneReader)
    : IRequestHandler<RemoveModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, currentUserAccessor))
        {
            return TenantSubscriptionItemBusinessRules.NotAuthorizedForTenant();
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);

        if (tenantSubscription is null)
        {
            return TenantSubscriptionItemBusinessRules.NoActiveSubscription();
        }

        var item = tenantSubscription.RenewingItems.FirstOrDefault(i => i.ModuleKey == request.ModuleKey);

        if (item is null)
        {
            return TenantSubscriptionItemBusinessRules.ModuleNotActive();
        }

        if (!tenantSubscription.CanRemoveModule(request.ModuleKey))
        {
            return TenantSubscriptionItemBusinessRules.CannotRemoveLastModule();
        }

        var providerSubscriptionItemId = item.ProviderSubscriptionItemId
            ?? throw new InvalidOperationException($"TenantSubscriptionItem {item.Id} has no ProviderSubscriptionItemId.");

        var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.RemoveSubscriptionItemAsync(providerSubscriptionItemId, ProrationBilling.None, cancellationToken: cancellationToken), TenantSubscriptionItemMessages.ModuleRemovalFailed);

        if (failure is not null)
        {
            return failure;
        }

        var endsAt = tenantSubscription.ScheduleModuleRemoval(request.ModuleKey, DateTime.UtcNow);

        _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        // The date as the business sees it, matching the admin UI.
        var zone = await timeZoneReader.GetAsync(request.TenantId, cancellationToken);
        var localEnd = new DateTimeOffset(endsAt, TimeSpan.Zero).InZone(zone);

        return Result.Success(string.Format(CultureInfo.InvariantCulture, TenantSubscriptionItemMessages.ModuleRemovalScheduledFormat, localEnd));
    }
}
