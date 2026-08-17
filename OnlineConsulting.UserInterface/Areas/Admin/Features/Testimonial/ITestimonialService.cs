using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Testimonial;

/// <summary>All Api orchestration for the Testimonial admin screens - TestimonialController only calls this and
/// renders the result, it never talks to IApiClient/IMediaService directly.</summary>
public interface ITestimonialService
{
    Task<List<TestimonialListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateTestimonialViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateTestimonialViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateTestimonialViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
