using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.DeletePromotion;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.UpdatePromotion;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public sealed class PromotionLinks() : ManagedContentLinks<PromotionResponse, UpdatePromotionCommand, DeletePromotionCommand>("UpdatePromotion", "DeletePromotion", resource => resource.Id);
