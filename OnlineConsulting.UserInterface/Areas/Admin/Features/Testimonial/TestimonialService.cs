using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Testimonial;

public class TestimonialService(IApiClient apiClient, IMediaService mediaService) : ITestimonialService
{
    private const string TestimonialsPath = "/api/site-content/testimonials";

    public async Task<List<TestimonialListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var testimonials = (await apiClient.GetAsync<List<TestimonialResponse>>(TestimonialsPath, cancellationToken)).ResultData ?? [];
        return testimonials.Select(t => new TestimonialListItemViewModel(t.Id, t.FirstName, t.LastName, t.Title, t.Description, t.ImageUrl)).ToList();
    }

    public async Task<UpdateTestimonialViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var testimonial = await FindAsync(id, cancellationToken);
        return testimonial is null
            ? null
            : new UpdateTestimonialViewModel
            {
                Id = testimonial.Id,
                FirstName = testimonial.FirstName,
                LastName = testimonial.LastName,
                Title = testimonial.Title,
                Description = testimonial.Description,
                ImageUrl = testimonial.ImageUrl,
            };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateTestimonialViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? string.Empty;
        return await apiClient.PostAsync(TestimonialsPath, new
        {
            model.FirstName,
            model.LastName,
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateTestimonialViewModel model, CancellationToken cancellationToken = default)
    {
        var imageUrl = await UploadAndResolveAsync(model.Image, cancellationToken) ?? (await FindAsync(model.Id, cancellationToken))?.ImageUrl ?? string.Empty;
        return await apiClient.PutAsync($"{TestimonialsPath}/{model.Id}", new
        {
            model.FirstName,
            model.LastName,
            model.Title,
            model.Description,
            ImageUrl = imageUrl,
        }, cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{TestimonialsPath}/{id}", cancellationToken);

    /// <summary>Testimonial's Api stores a plain ImageUrl string (not a MediaAssetId like Partnership) - upload
    /// then immediately resolve to a display Url so the wire contract still gets a plain string.</summary>
    private async Task<string?> UploadAndResolveAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var mediaAssetId = await mediaService.UploadAsync(file, cancellationToken);
        return mediaAssetId is null ? null : await mediaService.ResolveUrlAsync(mediaAssetId, cancellationToken);
    }

    private async Task<TestimonialResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<TestimonialResponse>>(TestimonialsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(t => t.Id == id);
    }

    private record TestimonialResponse(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder);
}
