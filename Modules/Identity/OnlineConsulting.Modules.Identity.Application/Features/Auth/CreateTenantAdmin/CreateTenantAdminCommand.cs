using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.CreateTenantAdmin;

/// <summary>Creates the first user of a brand-new tenant, called from OnlineConsulting.Api's Tenancy signup endpoint right after Tenancy's own ReserveTenantCommand succeeds and BEFORE Tenancy's ActivateTenantSubscriptionCommand runs (three separate ISender.Send calls from the Api layer, not one handler crossing module boundaries - see ReserveTenantCommand's doc comment). This ordering is deliberate: UserManager.CreateAsync's atomic unique index on email is the real, DB-enforced guard against duplicate signups, and it now runs before any billing happens, so a rejected duplicate email here never leaves an orphaned Stripe charge behind. Not reachable on its own route - no ISecureAddRequest, but only ever invoked server-side right after a fresh Tenant is created, never bound directly to a public request body.</summary>
public record CreateTenantAdminCommand(Guid TenantId, string FirstName, string LastName, string Email, string Password, string? PhoneNumber = null)
    : IRequest<OperationDataResult<CreateTenantAdminResult>>, ITransactionAddRequest;

public record CreateTenantAdminResult(Guid UserId);

public class CreateTenantAdminHandler(UserManager<User> userManager, IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<ConfirmEmailEmailModel> confirmEmailTemplate, IOptions<AuthEmailOptions> emailOptions)
    : IRequestHandler<CreateTenantAdminCommand, OperationDataResult<CreateTenantAdminResult>>
{
    public async Task<OperationDataResult<CreateTenantAdminResult>> Handle(CreateTenantAdminCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Email,
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

        // Enqueue before AddToRoleAsync - AddToRoleAsync's own SaveChangesAsync is what flushes this staged row.
        outboxWriter.Enqueue(user.Email ?? string.Empty, confirmEmailTemplate.Subject(confirmModel), confirmEmailTemplate.Build(confirmModel), sourceReference: $"User:{user.Id}");

        var roleResult = await userManager.AddToRoleAsync(user, GeneralOperationClaims.Admin);
        if (!roleResult.Succeeded)
        {
            return Result.Invalid<CreateTenantAdminResult>([.. roleResult.Errors.Select(e => e.Description)]);
        }

        return Result.Created(new CreateTenantAdminResult(user.Id), "The tenant admin account has been successfully created. Please check your email to confirm your account.");
    }
}
