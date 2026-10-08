using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Memberships.Application.Common.Templates;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Notifications;

/// <summary>Emails go through the Memberships outbox (retried by the email dispatcher); notifications go through the inbox-recording sender,
/// so the bell shows them even when no device is registered. Each carries "customerMembershipId" so tapping it opens the membership.</summary>
public class MembershipNotifier(
    IEmailOutboxWriter<IMembershipsOutboxModule> outboxWriter,
    IEmailTemplate<MembershipUpdateEmailModel> emailTemplate,
    IPushNotificationSender pushSender,
    IUserContactReader contactReader,
    IMembershipPlanRepository planRepository,
    ITenantTimeZoneReader timeZoneReader,
    ILogger<MembershipNotifier> logger) : IMembershipNotifier
{
    public async Task StartedAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        var plan = await PlanNameAsync(membership, cancellationToken);
        await PushAsync(membership, "Welcome to your membership", $"Your {plan} membership is active. Your member discounts apply from your next booking.", cancellationToken);
    }

    public async Task RenewedAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        var plan = await PlanNameAsync(membership, cancellationToken);
        await PushAsync(membership, "Membership renewed", $"Your {plan} membership renewed. Your receipt is in your email.", cancellationToken);
    }

    public async Task PaymentFailedAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        await EmailAsync(membership, MembershipUpdateKind.PaymentFailed, cancellationToken);
        await PushAsync(membership, "Membership payment failed",
            "We couldn't process your latest membership payment. Please update your payment method to avoid losing your benefits.", cancellationToken);
    }

    public async Task CancelledAfterFailuresAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        await EmailAsync(membership, MembershipUpdateKind.CancelledAfterFailures, cancellationToken);
        await PushAsync(membership, "Membership cancelled",
            "Your membership was cancelled after repeated payment failures. Subscribe again anytime to restore your benefits.", cancellationToken);
    }

    public async Task CancelledByStaffAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        await EmailAsync(membership, MembershipUpdateKind.CancelledByStaff, cancellationToken);
        await PushAsync(membership, "Membership cancelled", "Our team cancelled your membership, so it won't be charged again. Contact us if this is unexpected.", cancellationToken);
    }

    public async Task ReactivatedByStaffAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        await EmailAsync(membership, MembershipUpdateKind.ReactivatedByStaff, cancellationToken);
        await PushAsync(membership, "Membership active again", "Our team reactivated your membership. Your benefits continue as before.", cancellationToken);
    }

    private async Task<string> PlanNameAsync(CustomerMembership membership, CancellationToken cancellationToken)
    {
        try
        {
            var plan = await planRepository.GetAsync(p => p.Id == membership.MembershipPlanId, enableTracking: false, cancellationToken: cancellationToken);
            return plan?.Name ?? "membership";
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Membership {MembershipId}: reading the plan name failed.", membership.Id);
            return "membership";
        }
    }

    private async Task EmailAsync(CustomerMembership membership, MembershipUpdateKind kind, CancellationToken cancellationToken)
    {
        try
        {
            if (await contactReader.GetContactAsync(membership.UserId, cancellationToken) is not { Email: { Length: > 0 } email } contact)
            {
                logger.LogWarning("Membership {MembershipId}: no email on file for member {UserId}, skipping the {Kind} email.", membership.Id, membership.UserId, kind);
                return;
            }

            var zone = await timeZoneReader.GetAsync(membership.TenantId, cancellationToken);
            var model = new MembershipUpdateEmailModel(kind, contact.FirstName, await PlanNameAsync(membership, cancellationToken), membership.RenewalDate?.InZone(zone));
            await outboxWriter.EnqueueAsync(email, emailTemplate.Subject(model), emailTemplate.Build(model), sourceReference: $"Membership:{membership.Id}:{kind}",
                cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Membership {MembershipId}: queuing the {Kind} email failed.", membership.Id, kind);
        }
    }

    private async Task PushAsync(CustomerMembership membership, string title, string body, CancellationToken cancellationToken)
    {
        try
        {
            await pushSender.SendToUserAsync(membership.UserId, title, body, new Dictionary<string, string> { ["customerMembershipId"] = membership.Id.ToString() }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Membership {MembershipId}: sending '{Title}' failed.", membership.Id, title);
        }
    }
}
