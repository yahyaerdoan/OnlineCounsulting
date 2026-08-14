using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Categories.Application.Features.Rules;

public static class CategoryBusinessRules
{
    public static OperationResult CategoryNotFound(Guid categoryId) =>
        Result.NotFound(string.Format(CategoryMessages.CategoryNotFoundFormat, categoryId));
}
