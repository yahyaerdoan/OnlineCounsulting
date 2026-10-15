using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminCancelMembership;

/// <summary>Admin variant of CancelMembership - targets a specific CustomerMembership.Id (from the Membership
/// Subscribers admin list) instead of resolving the caller's own membership by UserId.</summary>
public record AdminCancelMembershipCommand(Guid MembershipId) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class AdminCancelMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway, IMembershipNotifier notifier,
    ITenantPaymentAccountReader paymentAccountReader) : IRequestHandler<AdminCancelMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AdminCancelMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.Id == request.MembershipId, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(string.Format(CustomerMembershipMessages.CustomerMembershipNotFoundFormat, request.MembershipId));
        }

        if (membership.IsCancelled)
        {
            return Result.Conflict(CustomerMembershipMessages.AlreadyCancelled);
        }

        if (membership.ProviderSubscriptionId is not null)
        {
            var paymentAccount = await paymentAccountReader.GetAsync(membership.TenantId, cancellationToken);
            _ = await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, connectedAccountId: paymentAccount.ConnectedAccountId, cancellationToken: cancellationToken);
        }

        membership.Cancel();

        _ = await repository.UpdateAsync(membership, cancellationToken: cancellationToken);

        await notifier.CancelledByStaffAsync(membership, cancellationToken);

        return Result.Success("Membership cancelled successfully.");
    }
}
