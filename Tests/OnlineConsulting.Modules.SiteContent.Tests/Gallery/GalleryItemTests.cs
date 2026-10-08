using OnlineConsulting.Modules.SiteContent.Domain.Gallery;

namespace OnlineConsulting.Modules.SiteContent.Tests.Gallery;

public class GalleryItemTests
{
    private static readonly Guid Cooling = Guid.NewGuid();
    private static readonly Guid Heating = Guid.NewGuid();
    private static readonly Guid Ducts = Guid.NewGuid();

    private static GalleryItem Create(params Guid[] categoryIds) => GalleryItem.Create("Before and after", null, 0, null, categoryIds);

    [Fact]
    public void Create_LinksEachCategoryOnce()
    {
        var item = Create(Cooling, Heating, Cooling);

        Assert.Equal([Cooling, Heating], item.CategoryIds);
        Assert.All(item.Categories, c => Assert.Equal(item.Id, c.GalleryItemId));
    }

    [Fact]
    public void Create_WithoutCategory_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => Create());
        _ = Assert.Throws<ArgumentException>(() => Create(Guid.Empty));
    }

    [Fact]
    public void Create_WithBlankDescription_Throws() =>
        Assert.ThrowsAny<ArgumentException>(() => GalleryItem.Create(" ", null, 0, null, [Cooling]));

    [Fact]
    public void Update_KeepsStillChosenLinksAndReplacesTheRest()
    {
        var item = Create(Cooling, Heating);
        var keptLink = item.Categories.Single(c => c.GalleryCategoryId == Cooling);

        item.Update("Updated", Guid.NewGuid(), 3, "{}", [Cooling, Ducts]);

        Assert.Equal([Cooling, Ducts], item.CategoryIds);
        Assert.Same(keptLink, item.Categories.Single(c => c.GalleryCategoryId == Cooling));
        Assert.Equal("Updated", item.Description);
        Assert.Equal(3, item.DisplayOrder);
    }

    [Fact]
    public void Update_WithSameCategories_ChangesNoLinks()
    {
        var item = Create(Cooling, Heating);
        var before = item.Categories.ToList();

        item.Update("Same tags", null, 0, null, [Heating, Cooling]);

        Assert.Equal(before, item.Categories);
    }

    [Fact]
    public void Update_WithoutCategory_ThrowsAndKeepsLinks()
    {
        var item = Create(Cooling);

        _ = Assert.Throws<ArgumentException>(() => item.Update("No tags", null, 0, null, []));
        Assert.Equal([Cooling], item.CategoryIds);
    }
}
