using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.CancelInvite;

public record CancelInviteCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [InvitesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, InvitesOperationClaims.Delete];
}

public class CancelInviteHandler(IInviteRepository inviteRepository, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<CancelInviteCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await inviteRepository.GetAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);
        if (invite is null)
        {
            return Result.NotFound(InviteMessages.InviteNotFound);
        }

        if (!TenantOwnershipGuard.CallerMayManage(invite.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden(InviteMessages.NotAuthorizedForOtherTenant);
        }

        if (invite.Status != InviteStatuses.Pending)
        {
            return Result.BadRequest(InviteMessages.InviteNotCancellable);
        }

        invite.Status = InviteStatuses.Revoked;
        _ = await inviteRepository.UpdateAsync(invite);

        return Result.Success(InviteMessages.InviteCancelled);
    }
}
