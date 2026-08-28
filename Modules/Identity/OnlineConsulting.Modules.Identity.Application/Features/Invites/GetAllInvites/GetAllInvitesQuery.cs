using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.GetAllInvites;

public record GetAllInvitesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<InviteResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [InvitesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, InvitesOperationClaims.Read];
}

public class GetAllInvitesHandler(IInviteRepository inviteRepository, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAllInvitesQuery, OperationDataResult<Paginate<InviteResponse>>>
{
    public async Task<OperationDataResult<Paginate<InviteResponse>>> Handle(GetAllInvitesQuery request, CancellationToken cancellationToken)
    {
        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        var isSuperAdmin = callerRoles.Contains(GlobalOperationClaims.SuperAdmin);

        var invitesQuery = isSuperAdmin
            ? inviteRepository.Query()
            : inviteRepository.Query().Where(i => i.TenantId == tenantProvider.TenantId);

        var pagedInvites = await invitesQuery.ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: i => i.CreatedDate, cancellationToken);

        var items = pagedInvites.Items.Select(i => new InviteResponse
        {
            Id = i.Id,
            Email = i.Email,
            RoleName = i.RoleName,
            Status = i.Status,
            ExpiresAt = i.ExpiresAt,
            CreatedDate = i.CreatedDate,
        }).ToList();

        return Result.Success(new Paginate<InviteResponse>
        {
            Items = items,
            Index = pagedInvites.Index,
            Size = pagedInvites.Size,
            Count = pagedInvites.Count,
            Pages = pagedInvites.Pages,
        }, "Invite data retrieved successfully.");
    }
}
