using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.GetLandingPage;

/// <summary>Public; NotFound until the landing copy is first saved.</summary>
public record GetLandingPageQuery : IRequest<OperationDataResult<LandingPageResponse>>;

public class GetLandingPageHandler(ILandingPageRepository repository) : IRequestHandler<GetLandingPageQuery, OperationDataResult<LandingPageResponse>>
{
    public async Task<OperationDataResult<LandingPageResponse>> Handle(GetLandingPageQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(_ => true, enableTracking: false, cancellationToken: cancellationToken);

        return entity is null
            ? Result.NotFound<LandingPageResponse>(SiteContentMessages.LandingPageNotSet)
            : Result.Success(LandingPageResponse.FromDomain(entity), "Landing page retrieved successfully.");
    }
}
