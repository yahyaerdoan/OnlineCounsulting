using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ValidateTenantAdmin;

/// <summary>Runs the admin account rules before a tenant signup charges the card, so a weak password is rejected while nothing has been paid.
/// The work is done by the validation pipeline (ValidateTenantAdminValidator); reaching the handler means the fields are valid.</summary>
public record ValidateTenantAdminQuery(string FirstName, string LastName, string Email, string Password) : IRequest<OperationResult>, ITenantAdminFields;

public class ValidateTenantAdminHandler : IRequestHandler<ValidateTenantAdminQuery, OperationResult>
{
    public Task<OperationResult> Handle(ValidateTenantAdminQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success("Account details look good."));
}
