using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.GetAllTestimonials;

public record GetAllTestimonialsQuery : IRequest<OperationDataResult<List<TestimonialResponse>>>;

public class GetAllTestimonialsHandler(ITestimonialRepository repository) : IRequestHandler<GetAllTestimonialsQuery, OperationDataResult<List<TestimonialResponse>>>
{
    public async Task<OperationDataResult<List<TestimonialResponse>>> Handle(GetAllTestimonialsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(TestimonialResponse.FromDomain).ToList();

        return Result.Success(response, "Testimonials retrieved successfully.");
    }
}
