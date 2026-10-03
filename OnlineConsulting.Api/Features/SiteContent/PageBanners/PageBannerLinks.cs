using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.DeletePageBanner;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.UpdatePageBanner;

namespace OnlineConsulting.Api.Features.SiteContent.PageBanners;

public sealed class PageBannerLinks() : ManagedContentLinks<PageBannerResponse, UpdatePageBannerCommand, DeletePageBannerCommand>("UpdatePageBanner", "DeletePageBanner", resource => resource.Id);
