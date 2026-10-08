using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.CancelInvite;

public record CancelInviteCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [InvitesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, InvitesOperationClaims.Delete];
}

public class CancelInviteHandler(IInviteRepository inviteRepository, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<CancelInviteCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await inviteRepository.GetAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);
        if (invite is null)
        {
            return Result.NotFound(InviteMessages.InviteNotFound);
        }

        if (!TenantOwnershipGuard.CallerMayManage(invite.TenantId, tenantProvider.TenantId, currentUserAccessor))
        {
            return Result.Forbidden(InviteMessages.NotAuthorizedForOtherTenant);
        }

        if (!invite.IsPending)
        {
            return Result.Conflict(InviteMessages.InviteNotCancellable);
        }

        invite.Revoke();
        _ = await inviteRepository.UpdateAsync(invite, cancellationToken: cancellationToken);

        return Result.Success(InviteMessages.InviteCancelled);
    }
}
