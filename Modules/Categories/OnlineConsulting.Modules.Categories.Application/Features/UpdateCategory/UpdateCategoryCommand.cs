using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Categories.Application.Features.UpdateCategory;

// See CreateCategoryCommand for why this doesn't opt into ITransactionAddRequest.
public record UpdateCategoryCommand(Guid Id, string Title, string Description, Guid ImgIconId)
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
            return Result.NotFound($"Category {request.Id} was not found.");

        category.Title = request.Title;
        category.Description = request.Description;
        category.ImgIconId = request.ImgIconId;

        await repository.UpdateAsync(category);

        return Result.Success("Category updated successfully.");
    }
}
