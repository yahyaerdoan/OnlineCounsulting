using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<OperationResult>, ITransactionAddRequest;

/// <summary>Always returns the same success message whether or not the email exists, to prevent account enumeration.</summary>
public class ForgotPasswordHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<ForgotPasswordEmailModel> template, IOptions<AuthEmailOptions> emailOptions)
    : IRequestHandler<ForgotPasswordCommand, OperationResult>
{
    private const string _successMessage = "If an account exists for that email, a reset link has been sent.";

    public async Task<OperationResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Success(_successMessage);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetUrl = $"{emailOptions.Value.ClientOrigin}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        var model = new ForgotPasswordEmailModel(user.FirstName, resetUrl);

        await outboxWriter.EnqueueAsync(user.Email ?? string.Empty, template.Subject(model), template.Build(model), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);

        return Result.Success(_successMessage);
    }
}
