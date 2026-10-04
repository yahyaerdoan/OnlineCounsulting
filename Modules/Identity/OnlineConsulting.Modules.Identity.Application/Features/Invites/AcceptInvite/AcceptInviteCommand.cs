using Core.CrossCuttingConcernLayer.Slugs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.AcceptInvite;

/// <summary>Accepts a teammate invite; not ISecureAddRequest since the invitee isn't logged in - the token itself is the proof of authorization.</summary>
public record AcceptInviteCommand(string Token, string FirstName, string LastName, string Password, string? PhoneNumber = null)
    : IRequest<OperationResult>, IIdentityTransactionRequest;

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

        if (!invite.IsPending)
        {
            return Result.Conflict(InviteMessages.InviteNotUsable);
        }

        var now = DateTime.UtcNow;
        if (invite.IsExpiredAt(now))
        {
            invite.Expire();

            _ = await inviteRepository.UpdateAsync(invite, cancellationToken: cancellationToken);

            return Result.Gone(InviteMessages.InviteExpired);
        }

        if (await userManager.FindByEmailAsync(invite.Email) is not null)
        {
            return Result.Conflict(InviteMessages.EmailAlreadyRegistered);
        }

        var userName = await SlugGenerator.GenerateUniqueAsync($"{request.FirstName} {request.LastName}",
            async prefix => await userManager.Users.Where(u => u.UserName != null && u.UserName.StartsWith(prefix)).Select(u => u.UserName ?? string.Empty).ToListAsync(cancellationToken));

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
            return OperationResult.Failure(createResult.ToFieldErrors(passwordField: nameof(request.Password)));
        }

        var roleResult = await userManager.AddToRoleAsync(user, invite.RoleName);
        if (!roleResult.Succeeded)
        {
            return Result.InternalServerError();
        }

        invite.Accept(now);

        _ = await inviteRepository.UpdateAsync(invite, cancellationToken: cancellationToken);

        return Result.Created($"Account created. Your username is \"{userName}\" - you can also sign in with your email.");
    }
}
