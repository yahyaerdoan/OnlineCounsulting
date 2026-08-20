using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using OnlineConsulting.Modules.Categories.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Categories.Application.Features.CreateCategory;

public record CreateCategoryCommand(string Title, string Description, string Icon, string? IconColor = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CategoriesOperationClaims.Admin, CategoriesOperationClaims.Write, CategoriesOperationClaims.Add];
}

public class CreateCategoryHandler(ICategoryRepository repository) : IRequestHandler<CreateCategoryCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Icon = request.Icon,
            IconColor = request.IconColor,
        };

        _ = await repository.AddAsync(category);

        return Result.Created(category.Id, "Category created successfully.");
    }
}
