using Core.CrossCuttingConcernLayer.Slugs;
using Core.PersistenceLayer.MultiTenancy;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>Creates a tenant's first user. Runs before billing so a rejected admin (e.g. a weak password) never leaves an orphaned charge. Server-side only, no public route.</summary>
public record CreateTenantAdminCommand(Guid TenantId, string FirstName, string LastName, string Email, string Password, string? PhoneNumber = null)
    : IRequest<OperationDataResult<CreateTenantAdminResult>>, IIdentityTransactionRequest, ITenantAdminFields;

public class CreateTenantAdminHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<ConfirmEmailEmailModel> confirmEmailTemplate, ITenantOriginReader originReader)
    : IRequestHandler<CreateTenantAdminCommand, OperationDataResult<CreateTenantAdminResult>>
{
    public async Task<OperationDataResult<CreateTenantAdminResult>> Handle(CreateTenantAdminCommand request, CancellationToken cancellationToken)
    {
        using var tenantScope = TenantScope.Begin(request.TenantId);

        var userName = await SlugGenerator.GenerateUniqueAsync($"{request.FirstName} {request.LastName}",
            async prefix => await userManager.Users.Where(u => u.UserName != null && u.UserName.StartsWith(prefix)).Select(u => u.UserName ?? string.Empty).ToListAsync(cancellationToken));

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
            return OperationDataResult<CreateTenantAdminResult>.Failure(createResult.ToFieldErrors(passwordField: nameof(request.Password), emailField: nameof(request.Email)));
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var origin = await originReader.GetOriginAsync(user.TenantId, cancellationToken);
        var confirmationUrl = $"{origin}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        var confirmModel = new ConfirmEmailEmailModel(user.FirstName, confirmationUrl);

        await outboxWriter.EnqueueAsync(user.Email ?? string.Empty, confirmEmailTemplate.Subject(confirmModel), confirmEmailTemplate.Build(confirmModel), sourceReference: $"User:{user.Id}", cancellationToken: cancellationToken);

        var roleResult = await userManager.AddToRoleAsync(user, GeneralOperationClaims.Admin);

        return !roleResult.Succeeded
            ? Result.InternalServerError<CreateTenantAdminResult>()
            : Result.Created(new CreateTenantAdminResult(user.Id),
            $"Account created. Your username is \"{userName}\" - you can also sign in with your email. Please check your email to confirm your account.");
    }
}
