using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CreateStripeConnectOnboardingLink;

/// <summary>Builds the Stripe Connect Standard OAuth consent URL for the caller's own tenant to redirect to. The tenant's own account is only
/// attached (see StripeConnectOAuthCallback) once they finish the OAuth flow and Stripe redirects back - this step never calls the provider.</summary>
public record CreateStripeConnectOnboardingLinkCommand(string ReturnUrl)
    : IRequest<OperationDataResult<StripeConnectOnboardingLinkResponse>>, ISecureAddRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class CreateStripeConnectOnboardingLinkHandler(ITenantProvider tenantProvider, IApiOriginReader apiOriginReader, IConnectAccountGateway connectAccountGateway,
    IStripeConnectStateProtector stateProtector)
    : IRequestHandler<CreateStripeConnectOnboardingLinkCommand, OperationDataResult<StripeConnectOnboardingLinkResponse>>
{
    public Task<OperationDataResult<StripeConnectOnboardingLinkResponse>> Handle(CreateStripeConnectOnboardingLinkCommand request, CancellationToken cancellationToken)
    {
        var redirectUri = $"{apiOriginReader.GetOrigin()}{StripeConnectOAuthRoutes.CallbackPath}";
        var state = stateProtector.Protect(tenantProvider.TenantId, request.ReturnUrl);

        var url = connectAccountGateway.BuildAuthorizeUrl(state, redirectUri);

        return Task.FromResult(Result.Success(new StripeConnectOnboardingLinkResponse(url), "Onboarding link created."));
    }
}
