using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminReactivateMembership;

/// <summary>Admin variant of ReactivateMembership for the Membership Subscribers list, e.g. a customer who changed their mind by phone.</summary>
public record AdminReactivateMembershipCommand(Guid MembershipId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class AdminReactivateMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway, IMembershipNotifier notifier, ITenantTimeZoneReader timeZoneReader) : IRequestHandler<AdminReactivateMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AdminReactivateMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.Id == request.MembershipId, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(string.Format(CustomerMembershipMessages.CustomerMembershipNotFoundFormat, request.MembershipId));
        }

        var result = await MembershipReactivation.RunAsync(membership, repository, subscriptionGateway, timeZoneReader, cancellationToken);
        if (result.IsSuccessful)
        {
            await notifier.ReactivatedByStaffAsync(membership, cancellationToken);
        }

        return result;
    }
}
