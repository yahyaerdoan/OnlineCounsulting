using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using OnlineConsulting.Modules.Categories.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Categories.Application.Features.DeleteCategory;

/// <summary>See CreateCategoryCommand for why this doesn't opt into ITransactionAddRequest.</summary>
public record DeleteCategoryCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CategoriesOperationClaims.Admin, CategoriesOperationClaims.Write, CategoriesOperationClaims.Delete];
}

public class DeleteCategoryHandler(ICategoryRepository repository) : IRequestHandler<DeleteCategoryCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

        if (category is null)
            return CategoryBusinessRules.CategoryNotFound(request.Id);

        await repository.DeleteAsync(category);

        return Result.Success("Category deleted successfully.");
    }
}
