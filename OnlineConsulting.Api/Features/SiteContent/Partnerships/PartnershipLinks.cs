using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.DeletePartnership;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.UpdatePartnership;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public sealed class PartnershipLinks() : ManagedContentLinks<PartnershipResponse, UpdatePartnershipCommand, DeletePartnershipCommand>("UpdatePartnership", "DeletePartnership", resource => resource.Id);
