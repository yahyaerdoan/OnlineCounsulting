using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;

public record RegisterCommand(string FirstName, string LastName, string UserName, string Email, string Password)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class RegisterHandler(UserManager<User> userManager, IEmailOutboxWriter outboxWriter, IEmailTemplate<ConfirmEmailEmailModel> confirmEmailTemplate, IOptions<AuthEmailOptions> emailOptions)
    : IRequestHandler<RegisterCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = TenantDefaults.DefaultTenantId,
            ImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png",
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result.Invalid([.. createResult.Errors.Select(e => e.Description)]);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationUrl = $"{emailOptions.Value.ClientOrigin}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        var confirmModel = new ConfirmEmailEmailModel(user.FirstName, confirmationUrl);

        outboxWriter.Enqueue(user.Email ?? string.Empty, confirmEmailTemplate.Subject(confirmModel), confirmEmailTemplate.Build(confirmModel), sourceReference: $"User:{user.Id}");

        return Result.Created("The user has been successfully created. Please check your email to confirm your account.");
    }
}
