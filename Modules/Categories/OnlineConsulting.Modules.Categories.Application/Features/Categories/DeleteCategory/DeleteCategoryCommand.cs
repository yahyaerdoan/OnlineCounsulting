using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Common;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Rules;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Categories.Application.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CategoriesOperationClaims.Admin, CategoriesOperationClaims.Write, CategoriesOperationClaims.Delete, GlobalOperationClaims.SuperAdmin];
}

public class DeleteCategoryHandler(ICategoryRepository repository) : IRequestHandler<DeleteCategoryCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

        if (category is null)
        {
            return CategoryBusinessRules.CategoryNotFound(request.Id);
        }

        _ = await repository.DeleteAsync(category, cancellationToken: cancellationToken);

        return Result.Success("Category deleted successfully.");
    }
}
