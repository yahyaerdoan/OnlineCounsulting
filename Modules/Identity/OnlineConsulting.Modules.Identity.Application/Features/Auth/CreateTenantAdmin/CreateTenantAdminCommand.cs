using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>Creates a tenant's first user. Runs before billing so a duplicate-email rejection never leaves an orphaned charge. Server-side only, no public route.</summary>
public record CreateTenantAdminCommand(Guid TenantId, string FirstName, string LastName, string Email, string Password, string? PhoneNumber = null)
    : IRequest<OperationDataResult<CreateTenantAdminResult>>, ITransactionAddRequest;

public record CreateTenantAdminResult(Guid UserId);

public class CreateTenantAdminHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<ConfirmEmailEmailModel> confirmEmailTemplate, IOptions<AuthEmailOptions> emailOptions)
    : IRequestHandler<CreateTenantAdminCommand, OperationDataResult<CreateTenantAdminResult>>
{
    public async Task<OperationDataResult<CreateTenantAdminResult>> Handle(CreateTenantAdminCommand request, CancellationToken cancellationToken)
    {
        var userName = await SlugGenerator.GenerateUniqueAsync($"{request.FirstName} {request.LastName}",
            async candidate => await userManager.FindByNameAsync(candidate) is not null);

        var user = new User
        {
            UserName = userName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = request.TenantId,
            PhoneNumber = request.PhoneNumber,
            ImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png",
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result.Invalid<CreateTenantAdminResult>([.. createResult.Errors.Select(e => e.Description)]);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationUrl = $"{emailOptions.Value.ClientOrigin}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        var confirmModel = new ConfirmEmailEmailModel(user.FirstName, confirmationUrl);

        await outboxWriter.EnqueueAsync(user.Email ?? string.Empty, confirmEmailTemplate.Subject(confirmModel), confirmEmailTemplate.Build(confirmModel), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);

        var roleResult = await userManager.AddToRoleAsync(user, GeneralOperationClaims.Admin);

        return !roleResult.Succeeded
            ? Result.Invalid<CreateTenantAdminResult>([.. roleResult.Errors.Select(e => e.Description)])
            : Result.Created(new CreateTenantAdminResult(user.Id),
            $"Account created. Your username is \"{userName}\" - you can also sign in with your email. Please check your email to confirm your account.");
    }
}
