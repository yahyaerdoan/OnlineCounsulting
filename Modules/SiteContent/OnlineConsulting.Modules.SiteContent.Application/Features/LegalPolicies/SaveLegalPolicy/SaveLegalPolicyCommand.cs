using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Legal;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.SaveLegalPolicy;

/// <summary>Creates the site's single policy row on first save, updates it afterwards.</summary>
public record SaveLegalPolicyCommand(string? PrivacyPolicy, string? TermsOfService) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class SaveLegalPolicyHandler(ILegalPolicyRepository repository) : IRequestHandler<SaveLegalPolicyCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SaveLegalPolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(_ => true, cancellationToken: cancellationToken);
        if (entity is null)
        {
            _ = await repository.AddAsync(Apply(new LegalPolicy(), request), cancellationToken: cancellationToken);
        }
        else
        {
            _ = await repository.UpdateAsync(Apply(entity, request), cancellationToken: cancellationToken);
        }

        return Result.Success(SiteContentMessages.LegalPolicySaved);
    }

    private static LegalPolicy Apply(LegalPolicy entity, SaveLegalPolicyCommand request)
    {
        entity.PrivacyPolicy = Normalize(request.PrivacyPolicy);
        entity.TermsOfService = Normalize(request.TermsOfService);
        return entity;
    }

    private static string? Normalize(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
