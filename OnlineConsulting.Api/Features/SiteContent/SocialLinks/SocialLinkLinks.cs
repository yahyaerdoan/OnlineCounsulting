using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.DeleteSocialLink;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.UpdateSocialLink;

namespace OnlineConsulting.Api.Features.SiteContent.SocialLinks;

public sealed class SocialLinkLinks() : ManagedContentLinks<SocialLinkResponse, UpdateSocialLinkCommand, DeleteSocialLinkCommand>("UpdateSocialLink", "DeleteSocialLink", resource => resource.Id);
