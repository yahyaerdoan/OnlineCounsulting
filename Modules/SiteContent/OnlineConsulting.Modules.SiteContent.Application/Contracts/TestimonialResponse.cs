using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Contracts;

public record TestimonialResponse(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static TestimonialResponse FromDomain(Testimonial entity) => new(entity.Id, entity.FirstName, entity.LastName, entity.Title, entity.Description, entity.ImageUrl, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
