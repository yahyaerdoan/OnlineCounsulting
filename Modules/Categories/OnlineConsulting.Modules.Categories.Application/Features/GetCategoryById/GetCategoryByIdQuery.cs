using MediatR;
using OnlineConsulting.Modules.Categories.Application.Contracts;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Categories.Application.Features.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<OperationDataResult<CategoryResponse>>;

public class GetCategoryByIdHandler(ICategoryRepository repository)
    : IRequestHandler<GetCategoryByIdQuery, OperationDataResult<CategoryResponse>>
{
    public async Task<OperationDataResult<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

        return category is null
            ? Result.NotFound<CategoryResponse>(string.Format(CategoryMessages.CategoryNotFoundFormat, request.Id))
            : Result.Success(CategoryResponse.FromDomain(category), "Category retrieved successfully.");
    }
}
