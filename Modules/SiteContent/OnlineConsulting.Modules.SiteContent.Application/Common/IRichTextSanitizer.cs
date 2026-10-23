namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Strips scripts, event handlers and unsafe links from rich-text HTML, keeping the editor's formatting.</summary>
public interface IRichTextSanitizer
{
    string Sanitize(string html);
}
