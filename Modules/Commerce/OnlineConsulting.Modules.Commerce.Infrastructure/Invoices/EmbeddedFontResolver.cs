using PdfSharp.Fonts;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Invoices;

/// <summary>Every family maps to the embedded Open Sans; bold and italic are simulated from the regular face.</summary>
internal sealed class EmbeddedFontResolver : IFontResolver
{
    public const string FamilyName = "Open Sans";
    private const string FaceName = "OpenSans-Regular";
    private static readonly Lazy<byte[]> FontBytes = new(LoadFont);

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic) => new(FaceName, bold, italic);

    public byte[]? GetFont(string faceName) => FontBytes.Value;

    private static byte[] LoadFont()
    {
        using var stream = typeof(EmbeddedFontResolver).Assembly.GetManifestResourceStream("Invoices.OpenSans-Regular.ttf")
            ?? throw new InvalidOperationException("The embedded invoice font is missing.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}
