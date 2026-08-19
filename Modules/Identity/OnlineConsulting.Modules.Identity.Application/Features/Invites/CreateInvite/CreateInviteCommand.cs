using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Encryptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.CreateInvite;

/// <summary>Invites a new teammate into the caller's own tenant by email. TenantId is always resolved
/// server-side from the caller's JWT claim via ITenantProvider, never accepted from the client, so a tenant
/// admin can only ever invite into their own tenant - there is no cross-tenant target to spoof. RoleName is
/// optional and defaults to GlobalOperationClaims.Member - the sensible default for "add a teammate"; callers
/// may still explicitly request GeneralOperationClaims.Admin, which is validated against RoleManager the same
/// way as any other role name, and GlobalOperationClaims.SuperAdmin remains explicitly rejected below.</summary>
public record CreateInviteCommand(string Email, string? RoleName = null) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [InvitesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, InvitesOperationClaims.Add];
}

public class CreateInviteHandler(IInviteRepository inviteRepository, RoleManager<Role> roleManager, UserManager<User> userManager, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor, IEmailOutboxWriter outboxWriter, IEmailTemplate<InviteEmailModel> inviteTemplate, IOptions<AuthEmailOptions> emailOptions)
    : IRequestHandler<CreateInviteCommand, OperationResult>
{
    private const int InviteValidityDays = 7;

    public async Task<OperationResult> Handle(CreateInviteCommand request, CancellationToken cancellationToken)
    {
        var requestedRoleName = string.IsNullOrWhiteSpace(request.RoleName) ? GlobalOperationClaims.Member : request.RoleName;

        if (string.Equals(requestedRoleName, GlobalOperationClaims.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            return Result.Forbidden(InviteMessages.RoleNotInvitable);

        var role = await roleManager.FindByNameAsync(requestedRoleName);
        if (role is null)
            return Result.BadRequest(InviteMessages.RoleNotFound);

        var tenantId = tenantProvider.TenantId;

        if (await userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(), cancellationToken))
            return Result.Conflict(InviteMessages.EmailAlreadyRegistered);

        var hasPendingInvite = await inviteRepository.AnyAsync(
            i => i.TenantId == tenantId && i.Email == request.Email && i.Status == InviteStatuses.Pending && i.ExpiresAt > DateTime.UtcNow,
            cancellationToken: cancellationToken);
        if (hasPendingInvite)
            return Result.Conflict(InviteMessages.InviteAlreadyPending);

        var invitedByUserId = Guid.TryParse(currentUserAccessor.UserId, out var parsedUserId)
            ? parsedUserId
            : throw new InvalidOperationException("Authenticated request is missing a valid user id claim.");

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = request.Email,
            Token = SecureTokenGenerator.GenerateUrlSafeToken(),
            RoleName = role.Name ?? requestedRoleName,
            ExpiresAt = DateTime.UtcNow.AddDays(InviteValidityDays),
            InvitedByUserId = invitedByUserId,
        };

        await inviteRepository.AddAsync(invite);

        var inviteUrl = $"{emailOptions.Value.ClientOrigin}/accept-invite?token={Uri.EscapeDataString(invite.Token)}";
        var inviteModel = new InviteEmailModel(inviteUrl);

        outboxWriter.Enqueue(invite.Email, inviteTemplate.Subject(inviteModel), inviteTemplate.Build(inviteModel), sourceReference: $"Invite:{invite.Id}");

        return Result.Created("The invite has been sent.");
    }
}
