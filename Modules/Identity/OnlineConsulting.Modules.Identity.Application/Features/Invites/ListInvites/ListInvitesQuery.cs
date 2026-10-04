using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.ListInvites;

public record ListInvitesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<InviteResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Invite.Email), nameof(Invite.Status), nameof(Invite.ExpiresAt)]);

    [JsonIgnore]
    public string[] Roles => [InvitesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, InvitesOperationClaims.Read];
}

public class ListInvitesHandler(IInviteRepository inviteRepository, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<ListInvitesQuery, OperationDataResult<Paginate<InviteResponse>>>
{
    public async Task<OperationDataResult<Paginate<InviteResponse>>> Handle(ListInvitesQuery request, CancellationToken cancellationToken)
    {
        var callerRoles = currentUserAccessor.Roles;
        var isSuperAdmin = callerRoles.Contains(GlobalOperationClaims.SuperAdmin);

        var invitesQuery = isSuperAdmin
            ? inviteRepository.Query()
            : inviteRepository.Query().Where(i => i.TenantId == tenantProvider.TenantId);

        var pagedInvites = await invitesQuery.ToDynamicPaginateAsync(request, defaultOrderBy: i => i.CreatedDate, tieBreaker: i => i.Id, cancellationToken: cancellationToken);

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
