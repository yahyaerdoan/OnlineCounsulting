using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetMyMembership;

public record GetMyMembershipQuery(Guid UserId) : IRequest<OperationDataResult<CustomerMembershipResponse>>;

public class GetMyMembershipHandler(ICustomerMembershipRepository repository)
    : IRequestHandler<GetMyMembershipQuery, OperationDataResult<CustomerMembershipResponse>>
{
    public async Task<OperationDataResult<CustomerMembershipResponse>> Handle(GetMyMembershipQuery request, CancellationToken cancellationToken)
    {
        var membership = await repository.GetAsync(m =>
            m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled,
            cancellationToken: cancellationToken);

        return membership is null
            ? Result.NotFound<CustomerMembershipResponse>(CustomerMembershipMessages.NoActiveMembership)
            : Result.Success(CustomerMembershipResponse.FromDomain(membership), "Membership retrieved successfully.");
    }
}
