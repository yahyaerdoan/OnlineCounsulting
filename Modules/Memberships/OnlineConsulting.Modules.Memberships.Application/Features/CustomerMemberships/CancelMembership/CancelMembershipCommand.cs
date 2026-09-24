using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.CancelMembership;

/// <summary>Cancels at period end (status flips via webhook, see OnSubscriptionCancelledHandler); AdminCancelMembershipCommand is the immediate-cancel override.</summary>
public record CancelMembershipCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CancelMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<CancelMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(CustomerMembershipMessages.NoActiveMembership);
        }

        if (membership.ProviderSubscriptionId is not null)
        {
            _ = await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, atPeriodEnd: true, cancellationToken: cancellationToken);
        }

        membership.CancelAtPeriodEnd = true;

        _ = await repository.UpdateAsync(membership);

        var renewalDate = membership.RenewalDate?.ToString("MMMM d, yyyy") ?? "the end of the current period";

        return Result.Success($"Your membership will remain active until {renewalDate} and won't renew after that.");
    }
}
