using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Gallery;

/// <summary>A gallery photo and the aggregate root of its category links: it always carries at least one category and links change only through it.</summary>
public class GalleryItem : SequentialGuidTenantEntity
{
    private readonly List<GalleryItemCategory> _categories = [];

    private GalleryItem()
    {
    }

    public string Description { get; private set; } = string.Empty;

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module's own DbContext.</summary>
    public Guid? PhotoMediaAssetId { get; private set; }

    public int DisplayOrder { get; private set; }

    /// <summary>Free-form JSON for template-specific extras (SiteContent convention).</summary>
    public string? Metadata { get; private set; }

    public IReadOnlyList<GalleryItemCategory> Categories => _categories;

    public IReadOnlyList<Guid> CategoryIds => [.. _categories.Select(c => c.GalleryCategoryId)];

    public static GalleryItem Create(string description, Guid? photoMediaAssetId, int displayOrder, string? metadata, IEnumerable<Guid> categoryIds)
    {
        var item = new GalleryItem();
        item.Update(description, photoMediaAssetId, displayOrder, metadata, categoryIds);
        return item;
    }

    /// <summary>Replaces the details and the category set; links still chosen are kept, the others removed and new ones added.</summary>
    public void Update(string description, Guid? photoMediaAssetId, int displayOrder, string? metadata, IEnumerable<Guid> categoryIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var wanted = categoryIds.ToHashSet();
        if (wanted.Count == 0 || wanted.Contains(Guid.Empty))
        {
            throw new ArgumentException("A gallery item needs at least one category.", nameof(categoryIds));
        }

        Description = description;
        PhotoMediaAssetId = photoMediaAssetId;
        DisplayOrder = displayOrder;
        Metadata = metadata;

        _ = _categories.RemoveAll(c => !wanted.Contains(c.GalleryCategoryId));

        var linked = _categories.Select(c => c.GalleryCategoryId).ToHashSet();
        _categories.AddRange(wanted.Where(id => !linked.Contains(id)).Select(id => GalleryItemCategory.Create(Id, id)));
    }
}
