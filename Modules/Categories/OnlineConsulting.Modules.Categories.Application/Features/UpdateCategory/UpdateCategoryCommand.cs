using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using OnlineConsulting.Modules.Categories.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Categories.Application.Features.UpdateCategory;

/// <summary>See CreateCategoryCommand for why this doesn't opt into ITransactionAddRequest.</summary>
public record UpdateCategoryCommand(Guid Id, string Title, string Description, string Icon, string? IconColor = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CategoriesOperationClaims.Admin, CategoriesOperationClaims.Write, CategoriesOperationClaims.Update];
}

public class UpdateCategoryHandler(ICategoryRepository repository) : IRequestHandler<UpdateCategoryCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
        {
            return CategoryBusinessRules.CategoryNotFound(request.Id);
        }

        category.Title = request.Title;
        category.Description = request.Description;
        category.Icon = request.Icon;
        category.IconColor = request.IconColor;

        _ = await repository.UpdateAsync(category);

        return Result.Success("Category updated successfully.");
    }
}
