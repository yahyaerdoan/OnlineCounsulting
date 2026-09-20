using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminCancelMembership;

/// <summary>Admin variant of CancelMembership - targets a specific CustomerMembership.Id (from the Membership
/// Subscribers admin list) instead of resolving the caller's own membership by UserId.</summary>
public record AdminCancelMembershipCommand(Guid MembershipId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class AdminCancelMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<AdminCancelMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AdminCancelMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.Id == request.MembershipId, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(string.Format(CustomerMembershipMessages.CustomerMembershipNotFoundFormat, request.MembershipId));
        }

        if (membership.Status == CustomerMembershipStatuses.Cancelled)
        {
            return Result.BadRequest(CustomerMembershipMessages.AlreadyCancelled);
        }

        if (membership.ProviderSubscriptionId is not null)
        {
            _ = await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, cancellationToken: cancellationToken);
        }

        membership.Status = CustomerMembershipStatuses.Cancelled;

        _ = await repository.UpdateAsync(membership);

        return Result.Success("Membership cancelled successfully.");
    }
}
