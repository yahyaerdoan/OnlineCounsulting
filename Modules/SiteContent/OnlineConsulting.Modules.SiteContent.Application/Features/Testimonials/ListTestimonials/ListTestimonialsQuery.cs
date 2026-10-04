using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.ListTestimonials;

public record ListTestimonialsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<TestimonialResponse>>>;

public class ListTestimonialsHandler(ITestimonialRepository repository)
    : IRequestHandler<ListTestimonialsQuery, OperationDataResult<Paginate<TestimonialResponse>>>
{
    public async Task<OperationDataResult<Paginate<TestimonialResponse>>> Handle(ListTestimonialsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<TestimonialResponse>
        {
            Items = [.. paged.Items.Select(TestimonialResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Testimonials retrieved successfully.");
    }
}
