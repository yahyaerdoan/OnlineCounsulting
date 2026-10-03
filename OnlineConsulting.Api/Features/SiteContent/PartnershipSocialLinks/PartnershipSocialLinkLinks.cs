using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.DeletePartnershipSocialLink;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.UpdatePartnershipSocialLink;

namespace OnlineConsulting.Api.Features.SiteContent.PartnershipSocialLinks;

public sealed class PartnershipSocialLinkLinks() : ManagedContentLinks<PartnershipSocialLinkResponse, UpdatePartnershipSocialLinkCommand, DeletePartnershipSocialLinkCommand>("UpdatePartnershipSocialLink", "DeletePartnershipSocialLink", resource => resource.Id);
