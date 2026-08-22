using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Promotion;

public class PromotionService(IApiClient apiClient) : IPromotionService
{
    private const string PromotionsPath = "/api/site-content/promotions";

    public async Task<List<PromotionListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var promotions = (await apiClient.GetAsync<List<PromotionResponse>>(PromotionsPath, cancellationToken)).ResultData ?? [];
        return promotions.Select(p => new PromotionListItemViewModel(p.Id, p.Title, p.Description, p.CtaText, p.CtaUrl, p.ExpiresAt, p.DisplayOrder)).ToList();
    }

    public async Task<UpdatePromotionViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await FindAsync(id, cancellationToken);
        return promotion is null
            ? null
            : new UpdatePromotionViewModel
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                CtaText = promotion.CtaText,
                CtaUrl = promotion.CtaUrl,
                ExpiresAt = promotion.ExpiresAt,
                DisplayOrder = promotion.DisplayOrder,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreatePromotionViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(PromotionsPath, new
        {
            model.Title,
            model.Description,
            model.CtaText,
            model.CtaUrl,
            model.ExpiresAt,
            model.DisplayOrder,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdatePromotionViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{PromotionsPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            model.CtaText,
            model.CtaUrl,
            model.ExpiresAt,
            model.DisplayOrder,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{PromotionsPath}/{id}", cancellationToken);

    private async Task<PromotionResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<PromotionResponse>>(PromotionsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(p => p.Id == id);
    }

    private record PromotionResponse(Guid Id, string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder);
}
