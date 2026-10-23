using Ganss.Xss;
using OnlineConsulting.Modules.SiteContent.Application.Common;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.RichText;

/// <summary>HtmlSanitizer with its safe defaults (allow-list of tags, attributes, CSS and URL schemes), plus the rich-text editor's
/// own ql-* classes (alignment, indent, size, code block) so formatting survives. Thread-safe once built.</summary>
public sealed class RichTextSanitizer : IRichTextSanitizer
{
    private static readonly string[] EditorClasses =
    [
        "ql-align-center", "ql-align-right", "ql-align-justify", "ql-direction-rtl", "ql-syntax",
        "ql-size-small", "ql-size-large", "ql-size-huge", "ql-font-serif", "ql-font-monospace",
        .. Enumerable.Range(1, 8).Select(level => $"ql-indent-{level}"),
    ];

    private readonly HtmlSanitizer _sanitizer = CreateSanitizer();

    public string Sanitize(string html) => _sanitizer.Sanitize(html);

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        _ = sanitizer.AllowedAttributes.Add("class");
        foreach (var editorClass in EditorClasses)
        {
            _ = sanitizer.AllowedClasses.Add(editorClass);
        }

        return sanitizer;
    }
}
