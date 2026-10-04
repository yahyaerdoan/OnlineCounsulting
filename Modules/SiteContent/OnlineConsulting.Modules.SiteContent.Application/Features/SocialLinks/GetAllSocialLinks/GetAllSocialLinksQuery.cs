using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.GetAllSocialLinks;

public record GetAllSocialLinksQuery : IRequest<OperationDataResult<List<SocialLinkResponse>>>;

public class GetAllSocialLinksHandler(ISocialLinkRepository repository) : IRequestHandler<GetAllSocialLinksQuery, OperationDataResult<List<SocialLinkResponse>>>
{
    public async Task<OperationDataResult<List<SocialLinkResponse>>> Handle(GetAllSocialLinksQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(SocialLinkResponse.FromDomain).ToList();

        return Result.Success(response, "Social links retrieved successfully.");
    }
}
