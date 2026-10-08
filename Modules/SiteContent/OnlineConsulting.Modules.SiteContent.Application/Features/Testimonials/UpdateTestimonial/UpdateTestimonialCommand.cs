using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.UpdateTestimonial;

public record UpdateTestimonialCommand(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateTestimonialHandler(ITestimonialRepository repository, IStorageService storageService) : IRequestHandler<UpdateTestimonialCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateTestimonialCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Testimonial", request.Id);
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ImageUrl = storageService.ToStoredUrl(request.ImageUrl);
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Testimonial updated successfully.");
    }
}
