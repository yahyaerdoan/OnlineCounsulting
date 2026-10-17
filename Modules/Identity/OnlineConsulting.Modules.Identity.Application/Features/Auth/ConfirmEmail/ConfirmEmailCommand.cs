using Core.PersistenceLayer.MultiTenancy;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ConfirmEmail;

/// <summary>Confirms a user's email from the link sent by ConfirmEmailTemplate.</summary>
public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<OperationResult>, IIdentityTransactionRequest;

public class ConfirmEmailHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<WelcomeEmailModel> welcomeTemplate, ITenantOriginReader originReader,
    ITenantOwnershipReader ownershipReader)
    : IRequestHandler<ConfirmEmailCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound(UserMessages.UserNotFound);
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            return Result.Success("Email already confirmed.");
        }

        var origin = await originReader.GetOriginAsync(user.TenantId, cancellationToken);
        var welcomeModel = new WelcomeEmailModel(user.FirstName, user.LastName, $"{origin}/login");

        var email = user.Email ?? string.Empty;

        // A tenant owner signed up with the platform, so their welcome comes from the platform too.
        var isOwner = await ownershipReader.IsOwnerAsync(user.TenantId, user.Id, cancellationToken);
        using (isOwner ? TenantScope.Begin(TenantDefaults.DefaultTenantId) : null)
        {
            await outboxWriter.EnqueueAsync(email, welcomeTemplate.Subject(welcomeModel), welcomeTemplate.Build(welcomeModel), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);
        }

        var confirmResult = await userManager.ConfirmEmailAsync(user, request.Token);

        return !confirmResult.Succeeded
            ? Result.BadRequest(string.Join("; ", confirmResult.Errors.Select(e => e.Description)))
            : Result.Success("Email confirmed successfully.");
    }
}
