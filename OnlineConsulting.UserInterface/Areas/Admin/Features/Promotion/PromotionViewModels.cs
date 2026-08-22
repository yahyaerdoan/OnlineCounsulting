using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Promotion;

public record PromotionListItemViewModel(Guid Id, string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder);

public class CreatePromotionViewModel
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? CtaText { get; set; }

    [MaxLength(500)]
    public string? CtaUrl { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public int DisplayOrder { get; set; }
}

public class UpdatePromotionViewModel : CreatePromotionViewModel
{
    public Guid Id { get; set; }
}
