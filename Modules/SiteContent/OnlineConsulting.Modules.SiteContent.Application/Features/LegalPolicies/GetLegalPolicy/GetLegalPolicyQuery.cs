using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Legal;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.GetLegalPolicy;

/// <summary>Public; NotFound until saved, or while the requested document (LegalDocuments) is empty.</summary>
public record GetLegalPolicyQuery(string? Document = null) : IRequest<OperationDataResult<LegalPolicyResponse>>;

public class GetLegalPolicyHandler(ILegalPolicyRepository repository) : IRequestHandler<GetLegalPolicyQuery, OperationDataResult<LegalPolicyResponse>>
{
    public async Task<OperationDataResult<LegalPolicyResponse>> Handle(GetLegalPolicyQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(_ => true, enableTracking: false, cancellationToken: cancellationToken);

        return entity is null || (request.Document is not null && Text(entity, request.Document) is null)
            ? Result.NotFound<LegalPolicyResponse>(SiteContentMessages.LegalPolicyNotSet)
            : Result.Success(LegalPolicyResponse.FromDomain(entity), SiteContentMessages.LegalPolicyRetrieved);
    }

    private static string? Text(LegalPolicy entity, string document) => document switch
    {
        LegalDocuments.PrivacyPolicy => entity.PrivacyPolicy,
        LegalDocuments.TermsOfService => entity.TermsOfService,
        _ => null,
    };
}
