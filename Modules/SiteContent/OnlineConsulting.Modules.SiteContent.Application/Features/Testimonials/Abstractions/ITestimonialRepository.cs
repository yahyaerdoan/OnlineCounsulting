using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;

public interface ITestimonialRepository : IAsyncRepository<Testimonial, Guid>
{
}
