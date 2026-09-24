using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.PauseMembership;

/// <summary>Stops billing indefinitely without cancelling - member keeps their pricing, no invoices until ResumeMembershipCommand.</summary>
public record PauseMembershipCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class PauseMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<PauseMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(PauseMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(CustomerMembershipMessages.NoActiveMembership);
        }

        if (membership.Status != CustomerMembershipStatuses.Active)
        {
            return Result.BadRequest(CustomerMembershipMessages.NotPausable);
        }

        if (membership.ProviderSubscriptionId is { } subscriptionId)
        {
            var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.PauseSubscriptionAsync(subscriptionId, cancellationToken), CustomerMembershipMessages.PauseFailed);

            if (failure is not null)
            {
                return failure;
            }
        }

        membership.Status = CustomerMembershipStatuses.Paused;

        _ = await repository.UpdateAsync(membership);

        return Result.Success("Membership paused successfully.");
    }
}
