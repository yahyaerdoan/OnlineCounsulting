using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.FindMyBusiness;

/// <summary>Emails the sign-in links of every business the address has an active account with. Looks across all tenants on purpose,
/// and always returns the same message so the response never reveals whether the address is registered.</summary>
public record FindMyBusinessCommand(string Email) : IRequest<OperationResult>;

public class FindMyBusinessHandler(UserManager<User> userManager, ITenantBrandReader brandReader, ITenantOriginReader originReader,
    IEmailOutboxWriter<IIdentityOutboxModule> outboxWriter, IEmailTemplate<FindMyBusinessEmailModel> template)
    : IRequestHandler<FindMyBusinessCommand, OperationResult>
{
    private const string _successMessage = "If that email has an account, we've sent it a list of where to sign in.";

    public async Task<OperationResult> Handle(FindMyBusinessCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);
        var tenantIds = await userManager.Users
            .Where(u => u.NormalizedEmail == normalizedEmail && u.IsActive && u.DeletedDate == null)
            .Select(u => u.TenantId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (tenantIds.Count == 0)
        {
            return Result.Success(_successMessage);
        }

        var businesses = new List<BusinessSignInLink>();
        foreach (var tenantId in tenantIds)
        {
            var brand = await brandReader.GetAsync(tenantId, cancellationToken);
            var origin = await originReader.GetOriginAsync(tenantId, cancellationToken);
            businesses.Add(new BusinessSignInLink(brand.Name, $"{origin}/login"));
        }

        var model = new FindMyBusinessEmailModel([.. businesses.OrderBy(b => b.BusinessName, StringComparer.OrdinalIgnoreCase)]);
        await outboxWriter.EnqueueAsync(request.Email, template.Subject(model), template.Build(model), sourceReference: "FindMyBusiness", cancellationToken: cancellationToken);

        return Result.Success(_successMessage);
    }
}
