using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Shared type scale for both themes: one font stack, semibold headings and sentence-case buttons.</summary>
public static class BrandTypography
{
    public static Typography Create()
    {
        var font = BrandPalette.FontStack;

        return new Typography
        {
            Default = new DefaultTypography { FontFamily = font },
            Body1 = new Body1Typography { FontFamily = font },
            Body2 = new Body2Typography { FontFamily = font },
            Button = new ButtonTypography { FontFamily = font, TextTransform = "none", FontWeight = "600" },
            H1 = new H1Typography { FontFamily = font, FontWeight = "600", LetterSpacing = "-0.02em" },
            H2 = new H2Typography { FontFamily = font, FontWeight = "600", LetterSpacing = "-0.02em" },
            H3 = new H3Typography { FontFamily = font, FontWeight = "600", LetterSpacing = "-0.02em" },
            H4 = new H4Typography { FontFamily = font, FontWeight = "600", LetterSpacing = "-0.01em" },
            H5 = new H5Typography { FontFamily = font, FontWeight = "600" },
            H6 = new H6Typography { FontFamily = font, FontWeight = "600" },
            Subtitle1 = new Subtitle1Typography { FontFamily = font, FontWeight = "600" },
            Subtitle2 = new Subtitle2Typography { FontFamily = font, FontWeight = "600" },
        };
    }
}
