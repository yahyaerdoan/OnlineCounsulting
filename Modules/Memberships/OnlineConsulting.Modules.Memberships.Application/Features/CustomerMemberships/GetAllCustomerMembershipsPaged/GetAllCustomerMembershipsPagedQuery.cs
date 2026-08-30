using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetAllCustomerMembershipsPaged;

public record GetAllCustomerMembershipsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<CustomerMembershipResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Read];
}

public class GetAllCustomerMembershipsPagedHandler(ICustomerMembershipRepository repository)
    : IRequestHandler<GetAllCustomerMembershipsPagedQuery, OperationDataResult<Paginate<CustomerMembershipResponse>>>
{
    public async Task<OperationDataResult<Paginate<CustomerMembershipResponse>>> Handle(GetAllCustomerMembershipsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: m => m.StartDate, tieBreaker: m => m.Id, cancellationToken);

        var response = new Paginate<CustomerMembershipResponse>
        {
            Items = [.. paged.Items.Select(CustomerMembershipResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Customer memberships retrieved successfully.");
    }
}
