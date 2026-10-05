using System.Text.RegularExpressions;

namespace OnlineConsulting.ArchitectureTests;

public partial class SourceLayoutTests
{
    private static readonly string[] Roots = ["Modules", "OnlineConsulting.Api"];

    [Fact]
    public void Each_file_declares_one_type_besides_its_accepted_companions()
    {
        var violations = Roots
            .SelectMany(root => Directory.EnumerateFiles(Path.Combine(Solution.RootDirectory(), root), "*.cs", SearchOption.AllDirectories))
            .Where(path => !IsGenerated(path))
            .Select(path => (Path: path, Extra: ExtraTypes(path)))
            .Where(file => file.Extra.Count > 0)
            .Select(file => $"{Path.GetRelativePath(Solution.RootDirectory(), file.Path)}: {string.Join(", ", file.Extra)}")
            .ToList();

        Assert.True(violations.Count == 0, $"One type per file (companions allowed: a request's handler, an endpoint's request DTOs, an email template's model and kind):\n{string.Join("\n", violations)}");
    }

    private static bool IsGenerated(string path)
    {
        var segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return segments.Contains("obj") || segments.Contains("bin") || segments.Contains("Migrations");
    }

    private static List<string> ExtraTypes(string path)
    {
        var main = Path.GetFileNameWithoutExtension(path);
        var stem = TemplateSuffix().Replace(main, string.Empty);

        return [.. TopLevelType().Matches(File.ReadAllText(path))
            .Select(match => match.Groups["name"].Value)
            .Where(name => name != main && !IsCompanion(name, main, stem))];
    }

    private static bool IsCompanion(string name, string main, string stem) =>
        name == HandlerOf(main)
        || name == main + "Handler"
        || name == main + "Endpoint"
        || name.EndsWith("Request", StringComparison.Ordinal)
        || (main.EndsWith("Template", StringComparison.Ordinal) && (name == stem + "Model" || name == stem + "EmailModel" || name == stem + "Kind"));

    private static string HandlerOf(string requestName) =>
        CommandOrQuerySuffix().Replace(requestName, string.Empty) + "Handler";

    [GeneratedRegex(@"^(?:public|internal)\s+(?:(?:sealed|static|abstract|partial|readonly|file)\s+)*(?:record\s+struct|record|class|interface|enum|struct)\s+(?<name>\w+)", RegexOptions.Multiline)]
    private static partial Regex TopLevelType();

    [GeneratedRegex("(Command|Query)$")]
    private static partial Regex CommandOrQuerySuffix();

    [GeneratedRegex("Template$")]
    private static partial Regex TemplateSuffix();
}
