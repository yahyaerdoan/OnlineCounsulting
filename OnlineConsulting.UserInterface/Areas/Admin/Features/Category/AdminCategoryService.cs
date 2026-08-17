using OnlineConsulting.UserInterface.Infrastructure.Api;
using SharedCategoryService = OnlineConsulting.UserInterface.Features.Category.ICategoryService;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Category;

public class AdminCategoryService(SharedCategoryService categoryService) : IAdminCategoryService
{
    /// <summary>The admin list screen has no pagination UI, so the whole set is pulled as one page (same
    /// approach already taken for the Message/Newsletter admin lists).</summary>
    private const int SinglePageSize = 1000;

    public async Task<List<CategoryListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryService.GetAllAsync(0, SinglePageSize, cancellationToken);
        return categories.Select(c => new CategoryListItemViewModel(c.Id, c.Title, c.Description, c.Icon, c.IconColor)).ToList();
    }

    public async Task<UpdateCategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return null;

        return new UpdateCategoryViewModel
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description,
            Icon = category.Icon,
            IconColor = category.IconColor,
        };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateCategoryViewModel model, CancellationToken cancellationToken = default) =>
        (await categoryService.CreateAsync(model.Title, model.Description, model.Icon, model.IconColor, cancellationToken)).WithoutData();

    public Task<ApiEnvelope> UpdateAsync(UpdateCategoryViewModel model, CancellationToken cancellationToken = default) =>
        categoryService.UpdateAsync(model.Id, model.Title, model.Description, model.Icon, model.IconColor, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        categoryService.DeleteAsync(id, cancellationToken);
}
