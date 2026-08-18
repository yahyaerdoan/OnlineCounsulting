using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Linq.Expressions;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.GetAllFaqItems;

/// <summary>ServiceId null returns every FAQ item (admin management view); set, it returns only that service's FAQs (service detail page).</summary>
public record GetAllFaqItemsQuery(Guid? ServiceId = null) : IRequest<OperationDataResult<List<FaqItemResponse>>>;

public class GetAllFaqItemsHandler(IFaqItemRepository repository) : IRequestHandler<GetAllFaqItemsQuery, OperationDataResult<List<FaqItemResponse>>>
{
    public async Task<OperationDataResult<List<FaqItemResponse>>> Handle(GetAllFaqItemsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<FaqItem, bool>>? predicate = request.ServiceId.HasValue
            ? item => item.ServiceId == request.ServiceId.Value
            : null;

        var entities = await repository.GetListAsync(predicate: predicate, orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(FaqItemResponse.FromDomain).ToList();

        return Result.Success(response, "FAQ items retrieved successfully.");
    }
}
