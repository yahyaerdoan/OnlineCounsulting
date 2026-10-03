using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.DeleteFeatureHighlight;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.UpdateFeatureHighlight;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlights;

public sealed class FeatureHighlightLinks() : ManagedContentLinks<FeatureHighlightResponse, UpdateFeatureHighlightCommand, DeleteFeatureHighlightCommand>("UpdateFeatureHighlight", "DeleteFeatureHighlight", resource => resource.Id);
