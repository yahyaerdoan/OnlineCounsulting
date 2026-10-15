using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Landing;

/// <summary>One item of a landing list section: a feature card, a how-it-works step or a FAQ entry.</summary>
public class LandingBlock : SequentialGuidTenantEntity
{
    /// <summary>A LandingSections value.</summary>
    public required string Section { get; set; }

    /// <summary>Card title, step title or FAQ question.</summary>
    public required string Title { get; set; }

    /// <summary>Card text, step text or FAQ answer.</summary>
    public required string Body { get; set; }

    /// <summary>MudBlazor icon value; unused by FAQ entries.</summary>
    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }
}
