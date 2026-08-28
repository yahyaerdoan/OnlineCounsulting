using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ConfirmEmail;

/// <summary>Confirms a user's email from the link sent by ConfirmEmailTemplate.</summary>
public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<OperationResult>, ITransactionAddRequest;

public class ConfirmEmailHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<WelcomeEmailModel> welcomeTemplate, IEmailTemplate<PolicyNoticeEmailModel> policyTemplate, IOptions<AuthEmailOptions> emailOptions)
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

        var welcomeModel = new WelcomeEmailModel(user.FirstName, user.LastName);

        var email = user.Email ?? string.Empty;

        await outboxWriter.EnqueueAsync(email, welcomeTemplate.Subject(welcomeModel), welcomeTemplate.Build(welcomeModel), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);

        var policyModel = new PolicyNoticeEmailModel(user.FirstName, $"{emailOptions.Value.ClientOrigin}/privacy-policy", $"{emailOptions.Value.ClientOrigin}/terms-of-service");

        await outboxWriter.EnqueueAsync(email, policyTemplate.Subject(policyModel), policyTemplate.Build(policyModel), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);

        var confirmResult = await userManager.ConfirmEmailAsync(user, request.Token);

        return !confirmResult.Succeeded
            ? Result.BadRequest(string.Join("; ", confirmResult.Errors.Select(e => e.Description)))
            : Result.Success("Email confirmed successfully.");
    }
}
