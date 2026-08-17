using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Contracts;

public record ServiceProcessStepResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static ServiceProcessStepResponse FromDomain(ServiceProcessStep entity) =>
        new(entity.Id, entity.Title, entity.Description, entity.Icon, entity.IconColor, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
