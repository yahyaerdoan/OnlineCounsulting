using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Category;

/// <summary>Category is Icon (CSS class string) + IconColor (hex string?) now, not an ImgIcon dropdown -
/// see Category's Api commands for the current field set.</summary>
public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync(int? index = null, int? size = null, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope<Guid>> CreateAsync(string title, string description, string icon, string? iconColor, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(Guid id, string title, string description, string icon, string? iconColor, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public record CategoryResponse(Guid Id, string Title, string Description, string Icon, string? IconColor);
