using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ReactivateMembership;

/// <summary>Reverses CancelMembershipCommand while the paid period is still running: the subscription renews again as before, with no new
/// charge. Once the period has ended (Status Cancelled) the member rejoins through SubscribeToMembership instead.</summary>
public record ReactivateMembershipCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [];
}

public class ReactivateMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway, ITenantTimeZoneReader timeZoneReader) : IRequestHandler<ReactivateMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ReactivateMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

        return membership is null
            ? Result.NotFound(CustomerMembershipMessages.NoActiveMembership)
            : await MembershipReactivation.RunAsync(membership, repository, subscriptionGateway, timeZoneReader, cancellationToken);
    }
}
