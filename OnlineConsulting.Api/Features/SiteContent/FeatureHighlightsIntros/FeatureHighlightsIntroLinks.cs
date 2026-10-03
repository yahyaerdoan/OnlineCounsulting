using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.DeleteFeatureHighlightsIntro;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.UpdateFeatureHighlightsIntro;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public sealed class FeatureHighlightsIntroLinks() : ManagedContentLinks<FeatureHighlightsIntroResponse, UpdateFeatureHighlightsIntroCommand, DeleteFeatureHighlightsIntroCommand>("UpdateFeatureHighlightsIntro", "DeleteFeatureHighlightsIntro", resource => resource.Id);
