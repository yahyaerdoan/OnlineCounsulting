using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.DeleteAboutUs;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.UpdateAboutUs;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public sealed class AboutUsLinks() : ManagedContentLinks<AboutUsResponse, UpdateAboutUsCommand, DeleteAboutUsCommand>("UpdateAboutUs", "DeleteAboutUs", resource => resource.Id);
