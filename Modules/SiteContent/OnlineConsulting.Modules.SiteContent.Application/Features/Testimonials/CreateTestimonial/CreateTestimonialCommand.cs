using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.CreateTestimonial;

public record CreateTestimonialCommand(string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateTestimonialHandler(ITestimonialRepository repository) : IRequestHandler<CreateTestimonialCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateTestimonialCommand request, CancellationToken cancellationToken)
    {
        var entity = new Testimonial
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Testimonial created successfully.");
    }
}
