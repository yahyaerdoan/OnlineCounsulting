using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.AcceptInvite;

/// <summary>Accepts a teammate invite and creates the invited person's account. Deliberately not an
/// ISecureAddRequest - the invited person is not logged in yet, and the invite token itself is the only
/// proof of authorization needed. TenantId comes from the Invite row, never from the caller.</summary>
public record AcceptInviteCommand(string Token, string FirstName, string LastName, string Password, string? PhoneNumber = null)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class AcceptInviteHandler(IInviteRepository inviteRepository, UserManager<User> userManager)
    : IRequestHandler<AcceptInviteCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await inviteRepository.GetAsync(i => i.Token == request.Token, cancellationToken: cancellationToken);
        if (invite is null)
        {
            return Result.NotFound(InviteMessages.InviteNotFound);
        }

        if (invite.Status != InviteStatuses.Pending)
        {
            return Result.BadRequest(InviteMessages.InviteNotUsable);
        }

        if (invite.ExpiresAt < DateTime.UtcNow)
        {
            invite.Status = InviteStatuses.Expired;
            _ = await inviteRepository.UpdateAsync(invite);
            return Result.BadRequest(InviteMessages.InviteExpired);
        }

        if (await userManager.FindByEmailAsync(invite.Email) is not null)
        {
            return Result.Conflict(InviteMessages.EmailAlreadyRegistered);
        }

        var userName = await SlugGenerator.GenerateUniqueAsync($"{request.FirstName} {request.LastName}",
            async candidate => await userManager.FindByNameAsync(candidate) is not null);

        var user = new User
        {
            UserName = userName,
            Email = invite.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = invite.TenantId,
            EmailConfirmed = true,
            PhoneNumber = request.PhoneNumber,
            ImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png",
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result.Invalid([.. createResult.Errors.Select(e => e.Description)]);
        }

        var roleResult = await userManager.AddToRoleAsync(user, invite.RoleName);
        if (!roleResult.Succeeded)
        {
            return Result.Invalid([.. roleResult.Errors.Select(e => e.Description)]);
        }

        invite.Status = InviteStatuses.Accepted;
        invite.AcceptedAt = DateTime.UtcNow;
        _ = await inviteRepository.UpdateAsync(invite);

        return Result.Created($"Account created. Your username is \"{userName}\" - you can also sign in with your email.");
    }
}
