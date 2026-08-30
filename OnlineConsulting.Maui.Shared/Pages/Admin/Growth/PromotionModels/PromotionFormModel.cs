namespace OnlineConsulting.Maui.Shared.Pages.Admin.Growth.PromotionModels;

public class PromotionFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CtaText { get; set; }

    public string? CtaUrl { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int DisplayOrder { get; set; }
}
