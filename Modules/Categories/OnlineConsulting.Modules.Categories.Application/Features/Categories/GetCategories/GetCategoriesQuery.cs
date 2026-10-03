using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Categories.Application.Features.Categories.GetCategories;

public record GetCategoriesQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<CategoryResponse>>>;

public class GetCategoriesHandler(ICategoryRepository repository) : IRequestHandler<GetCategoriesQuery, OperationDataResult<Paginate<CategoryResponse>>>
{
    public async Task<OperationDataResult<Paginate<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetListAsync(orderBy: q => q.OrderBy(c => c.Id), index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<CategoryResponse>
        {
            Items = [.. categories.Items.Select(CategoryResponse.FromDomain)],
            Index = categories.Index,
            Size = categories.Size,
            Count = categories.Count,
            Pages = categories.Pages,
        };

        return Result.Success(response, "Categories retrieved successfully.");
    }
}
