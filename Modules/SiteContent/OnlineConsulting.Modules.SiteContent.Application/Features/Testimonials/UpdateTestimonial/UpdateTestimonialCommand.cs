using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.UpdateTestimonial;

public record UpdateTestimonialCommand(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateTestimonialHandler(ITestimonialRepository repository) : IRequestHandler<UpdateTestimonialCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateTestimonialCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Testimonial", request.Id);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ImageUrl = request.ImageUrl;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        return Result.Success("Testimonial updated successfully.");
    }
}
