using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ResumeMembership;

/// <summary>Reverses PauseMembershipCommand - billing resumes. RenewalDate self-corrects on the next
/// real provider renewal webhook rather than being guessed here. UserId is always resolved
/// server-side, never trusted from the client.</summary>
public record ResumeMembershipCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class ResumeMembershipHandler(ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<ResumeMembershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ResumeMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m => m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(CustomerMembershipMessages.NoActiveMembership);
        }

        if (membership.Status != CustomerMembershipStatuses.Paused)
        {
            return Result.BadRequest(CustomerMembershipMessages.NotResumable);
        }

        if (membership.ProviderSubscriptionId is { } subscriptionId)
        {
            var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.ResumeSubscriptionAsync(subscriptionId, cancellationToken), CustomerMembershipMessages.ResumeFailed);

            if (failure is not null)
            {
                return failure;
            }
        }

        membership.Status = CustomerMembershipStatuses.Active;

        _ = await repository.UpdateAsync(membership);

        return Result.Success("Membership resumed successfully.");
    }
}
