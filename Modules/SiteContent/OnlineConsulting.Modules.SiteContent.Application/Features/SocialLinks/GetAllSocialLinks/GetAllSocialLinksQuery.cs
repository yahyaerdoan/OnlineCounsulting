using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.GetAllSocialLinks;

public record GetAllSocialLinksQuery : IRequest<OperationDataResult<List<SocialLinkResponse>>>;

public class GetAllSocialLinksHandler(ISocialLinkRepository repository) : IRequestHandler<GetAllSocialLinksQuery, OperationDataResult<List<SocialLinkResponse>>>
{
    public async Task<OperationDataResult<List<SocialLinkResponse>>> Handle(GetAllSocialLinksQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(SocialLinkResponse.FromDomain).ToList();

        return Result.Success(response, "Social links retrieved successfully.");
    }
}
