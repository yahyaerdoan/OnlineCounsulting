using OnlineConsulting.Modules.SiteContent.Infrastructure.RichText;

namespace OnlineConsulting.Modules.SiteContent.Tests.RichText;

public class RichTextSanitizerTests
{
    private readonly RichTextSanitizer _sanitizer = new();

    [Fact]
    public void Sanitize_KeepsEditorFormatting()
    {
        const string html = "<h2>Who we are</h2><p class=\"ql-align-center\"><strong>Bold</strong> and <em>italic</em></p><ul><li>One</li></ul><p><a href=\"https://example.com\">Link</a></p>";

        Assert.Equal(html, _sanitizer.Sanitize(html));
    }

    [Theory]
    [InlineData("<p>Hi</p><script>alert(1)</script>", "<p>Hi</p>")]
    [InlineData("<img src=\"x\" onerror=\"alert(1)\">", "<img src=\"x\">")]
    [InlineData("<a href=\"javascript:alert(1)\">x</a>", "<a>x</a>")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe><p>ok</p>", "<p>ok</p>")]
    public void Sanitize_RemovesScriptsAndHandlers(string html, string expected) =>
        Assert.Equal(expected, _sanitizer.Sanitize(html));

    [Fact]
    public void Sanitize_KeepsOnlyEditorClasses() =>
        Assert.Equal("<p class=\"ql-indent-2\">x</p>", _sanitizer.Sanitize("<p class=\"ql-indent-2 marketing-hero\">x</p>"));

    [Fact]
    public void Sanitize_IsStable()
    {
        var once = _sanitizer.Sanitize("<p onclick=\"x()\">Text</p>");

        Assert.Equal(once, _sanitizer.Sanitize(once));
    }
}
