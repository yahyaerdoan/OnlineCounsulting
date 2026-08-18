using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetAllCustomerMemberships;

public record GetAllCustomerMembershipsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<CustomerMembershipResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Read];
}

public class GetAllCustomerMembershipsHandler(ICustomerMembershipRepository repository)
    : IRequestHandler<GetAllCustomerMembershipsQuery, OperationDataResult<Paginate<CustomerMembershipResponse>>>
{
    public async Task<OperationDataResult<Paginate<CustomerMembershipResponse>>> Handle(GetAllCustomerMembershipsQuery request, CancellationToken cancellationToken)
    {
        var memberships = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<CustomerMembershipResponse>
        {
            Items = [.. memberships.Items.Select(CustomerMembershipResponse.FromDomain)],
            Index = memberships.Index,
            Size = memberships.Size,
            Count = memberships.Count,
            Pages = memberships.Pages,
        };

        return Result.Success(response, "Customer memberships retrieved successfully.");
    }
}
