using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.DeleteHeroSlide;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.UpdateHeroSlide;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public sealed class HeroSlideLinks() : ManagedContentLinks<HeroSlideResponse, UpdateHeroSlideCommand, DeleteHeroSlideCommand>("UpdateHeroSlide", "DeleteHeroSlide", resource => resource.Id);
