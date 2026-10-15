using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.SaveLandingPage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingPages;

public class SaveLandingPage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/landing-page", Handle)
            .WithTags("SiteContent/LandingPage")
            .RequireAuthorization()
            .WithName("SaveLandingPage")
            .WithDescription("Creates or updates the platform landing copy (SuperAdmin).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] SaveLandingPageRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SaveLandingPageRequest(
    string HeroEyebrow, string HeroTitle, string HeroLead, string PrimaryCtaText, string SecondaryCtaText,
    string FeaturesEyebrow, string FeaturesTitle, string? FeaturesLead,
    string StepsEyebrow, string StepsTitle, string? StepsLead,
    string PricingEyebrow, string PricingTitle, string? PricingLead,
    string FaqEyebrow, string FaqTitle,
    string ClosingTitle, string? ClosingLead,
    string? AboutHeroEyebrow = null, string? AboutHeroTitle = null, string? AboutHeroLead = null,
    string? IndustriesEyebrow = null, string? IndustriesTitle = null, string? IndustriesLead = null,
    string? ContactHeroEyebrow = null, string? ContactHeroTitle = null, string? ContactHeroLead = null,
    string? HeroImageUrl = null, string? FeaturesHeroImageUrl = null, string? IndustriesHeroImageUrl = null,
    string? AboutHeroImageUrl = null, string? ContactHeroImageUrl = null)
{
    public SaveLandingPageCommand ToCommand() => new(
        HeroEyebrow, HeroTitle, HeroLead, PrimaryCtaText, SecondaryCtaText,
        FeaturesEyebrow, FeaturesTitle, FeaturesLead,
        StepsEyebrow, StepsTitle, StepsLead,
        PricingEyebrow, PricingTitle, PricingLead,
        FaqEyebrow, FaqTitle,
        ClosingTitle, ClosingLead,
        AboutHeroEyebrow, AboutHeroTitle, AboutHeroLead,
        IndustriesEyebrow, IndustriesTitle, IndustriesLead,
        ContactHeroEyebrow, ContactHeroTitle, ContactHeroLead,
        HeroImageUrl, FeaturesHeroImageUrl, IndustriesHeroImageUrl,
        AboutHeroImageUrl, ContactHeroImageUrl);
}
