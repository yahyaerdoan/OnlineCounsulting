using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.DeleteLandingBlock;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.UpdateLandingBlock;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public sealed class LandingBlockLinks() : ManagedContentLinks<LandingBlockResponse, UpdateLandingBlockCommand, DeleteLandingBlockCommand>("UpdateLandingBlock", "DeleteLandingBlock", resource => resource.Id);
