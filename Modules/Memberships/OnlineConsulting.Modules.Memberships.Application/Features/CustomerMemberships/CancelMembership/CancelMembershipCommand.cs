using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.CancelMembership;

/// <summary>Cancels immediately (no cancel-at-period-end in this phase) - UserId is always resolved server-side, never trusted from the client.</summary>
public record CancelMembershipCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CancelMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<CancelMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m =>
            m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled,
            cancellationToken: cancellationToken);

        if (membership is null)
            return Result.NotFound(CustomerMembershipMessages.NoActiveMembership);

        if (membership.ProviderSubscriptionId is not null)
            await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, cancellationToken);

        membership.Status = CustomerMembershipStatuses.Cancelled;
        await repository.UpdateAsync(membership);

        return Result.Success("Membership cancelled successfully.");
    }
}
