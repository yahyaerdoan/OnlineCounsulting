using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.GetAllTestimonials;

public record GetAllTestimonialsQuery : IRequest<OperationDataResult<List<TestimonialResponse>>>;

public class GetAllTestimonialsHandler(ITestimonialRepository repository) : IRequestHandler<GetAllTestimonialsQuery, OperationDataResult<List<TestimonialResponse>>>
{
    public async Task<OperationDataResult<List<TestimonialResponse>>> Handle(GetAllTestimonialsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(TestimonialResponse.FromDomain).ToList();

        return Result.Success(response, "Testimonials retrieved successfully.");
    }
}
