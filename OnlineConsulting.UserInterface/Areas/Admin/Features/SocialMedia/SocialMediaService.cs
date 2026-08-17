using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

public class SocialMediaService(IApiClient apiClient) : ISocialMediaService
{
    private const string SocialLinksPath = "/api/site-content/social-links";

    public async Task<List<SocialMediaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var links = (await apiClient.GetAsync<List<SocialLinkResponse>>(SocialLinksPath, cancellationToken)).ResultData ?? [];
        return links.Select(l => new SocialMediaListItemViewModel(l.Id, l.Name, l.Url, l.Icon, l.IconColor, l.DisplayOrder)).ToList();
    }

    public async Task<UpdateSocialMediaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var link = await FindAsync(id, cancellationToken);
        if (link is null)
            return null;

        return new UpdateSocialMediaViewModel
        {
            Id = link.Id,
            Name = link.Name,
            Url = link.Url,
            Icon = link.Icon,
            IconColor = link.IconColor,
            DisplayOrder = link.DisplayOrder,
        };
    }

    public Task<ApiEnvelope> CreateAsync(CreateSocialMediaViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(SocialLinksPath, new { model.Name, model.Url, model.Icon, model.IconColor, model.DisplayOrder }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateSocialMediaViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{SocialLinksPath}/{model.Id}", new { model.Name, model.Url, model.Icon, model.IconColor, model.DisplayOrder }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{SocialLinksPath}/{id}", cancellationToken);

    private async Task<SocialLinkResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<SocialLinkResponse>>(SocialLinksPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(l => l.Id == id);
    }

    private record SocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor, int DisplayOrder);
}
