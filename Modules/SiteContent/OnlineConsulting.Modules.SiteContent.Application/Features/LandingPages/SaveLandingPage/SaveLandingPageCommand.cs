using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.SaveLandingPage;

/// <summary>Creates the single landing row on first save, updates it afterwards.</summary>
public record SaveLandingPageCommand(
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
    string? AboutHeroImageUrl = null, string? ContactHeroImageUrl = null) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => LandingAccess.Roles;

    /// <summary>Platform-level; a tenant admin must never reach this.</summary>
    public bool AllowTenantBypass => false;
}

public class SaveLandingPageHandler(ILandingPageRepository repository, IStorageService storageService) : IRequestHandler<SaveLandingPageCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SaveLandingPageCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(_ => true, cancellationToken: cancellationToken);

        if (entity is null)
        {
            _ = await repository.AddAsync(Apply(new LandingPage
            {
                HeroEyebrow = request.HeroEyebrow,
                HeroTitle = request.HeroTitle,
                HeroLead = request.HeroLead,
                PrimaryCtaText = request.PrimaryCtaText,
                SecondaryCtaText = request.SecondaryCtaText,
                FeaturesEyebrow = request.FeaturesEyebrow,
                FeaturesTitle = request.FeaturesTitle,
                StepsEyebrow = request.StepsEyebrow,
                StepsTitle = request.StepsTitle,
                PricingEyebrow = request.PricingEyebrow,
                PricingTitle = request.PricingTitle,
                FaqEyebrow = request.FaqEyebrow,
                FaqTitle = request.FaqTitle,
                ClosingTitle = request.ClosingTitle,
            }, request), cancellationToken: cancellationToken);
        }
        else
        {
            _ = await repository.UpdateAsync(Apply(entity, request), cancellationToken: cancellationToken);
        }

        return Result.Success("Landing page saved successfully.");
    }

    private LandingPage Apply(LandingPage entity, SaveLandingPageCommand request)
    {
        entity.HeroEyebrow = request.HeroEyebrow;
        entity.HeroTitle = request.HeroTitle;
        entity.HeroLead = request.HeroLead;
        entity.PrimaryCtaText = request.PrimaryCtaText;
        entity.SecondaryCtaText = request.SecondaryCtaText;
        entity.FeaturesEyebrow = request.FeaturesEyebrow;
        entity.FeaturesTitle = request.FeaturesTitle;
        entity.FeaturesLead = request.FeaturesLead;
        entity.StepsEyebrow = request.StepsEyebrow;
        entity.StepsTitle = request.StepsTitle;
        entity.StepsLead = request.StepsLead;
        entity.PricingEyebrow = request.PricingEyebrow;
        entity.PricingTitle = request.PricingTitle;
        entity.PricingLead = request.PricingLead;
        entity.FaqEyebrow = request.FaqEyebrow;
        entity.FaqTitle = request.FaqTitle;
        entity.ClosingTitle = request.ClosingTitle;
        entity.ClosingLead = request.ClosingLead;
        entity.AboutHeroEyebrow = request.AboutHeroEyebrow;
        entity.AboutHeroTitle = request.AboutHeroTitle;
        entity.AboutHeroLead = request.AboutHeroLead;
        entity.IndustriesEyebrow = request.IndustriesEyebrow;
        entity.IndustriesTitle = request.IndustriesTitle;
        entity.IndustriesLead = request.IndustriesLead;
        entity.ContactHeroEyebrow = request.ContactHeroEyebrow;
        entity.ContactHeroTitle = request.ContactHeroTitle;
        entity.ContactHeroLead = request.ContactHeroLead;
        entity.HeroImageUrl = storageService.ToStoredUrl(request.HeroImageUrl);
        entity.FeaturesHeroImageUrl = storageService.ToStoredUrl(request.FeaturesHeroImageUrl);
        entity.IndustriesHeroImageUrl = storageService.ToStoredUrl(request.IndustriesHeroImageUrl);
        entity.AboutHeroImageUrl = storageService.ToStoredUrl(request.AboutHeroImageUrl);
        entity.ContactHeroImageUrl = storageService.ToStoredUrl(request.ContactHeroImageUrl);
        return entity;
    }
}
