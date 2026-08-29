using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Categories.Application.Features.GetAllCategoriesPaged;

/// <summary>Sortable/filterable variant of GetCategoriesQuery for the admin ServerDataTable.</summary>
public record GetAllCategoriesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<CategoryResponse>>>;

public class GetAllCategoriesPagedHandler(ICategoryRepository repository)
    : IRequestHandler<GetAllCategoriesPagedQuery, OperationDataResult<Paginate<CategoryResponse>>>
{
    public async Task<OperationDataResult<Paginate<CategoryResponse>>> Handle(GetAllCategoriesPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: c => c.Title, tieBreaker: c => c.Id, cancellationToken);

        var response = new Paginate<CategoryResponse>
        {
            Items = [.. paged.Items.Select(CategoryResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Categories retrieved successfully.");
    }
}
