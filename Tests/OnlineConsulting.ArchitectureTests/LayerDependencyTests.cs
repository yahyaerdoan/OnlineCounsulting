namespace OnlineConsulting.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly string[] PresentationAndPersistence = ["Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore", "Hateoas", "OnlineConsulting.Api"];

    private static readonly string[] Frameworks = [.. PresentationAndPersistence, "MediatR", "FluentValidation", "ResultHandler"];

    [Fact]
    public void Domain_depends_on_no_framework_and_no_outer_layer() =>
        AssertNoDependency("Domain", Frameworks, ".Application", ".Infrastructure");

    [Fact]
    public void Application_knows_nothing_about_http_json_links_or_infrastructure() =>
        AssertNoDependency("Application", ["Microsoft.AspNetCore", "Hateoas", "OnlineConsulting.Api"], ".Infrastructure");

    [Fact]
    public void Infrastructure_never_reaches_up_to_the_api() =>
        AssertNoDependency("Infrastructure", ["OnlineConsulting.Api", "Hateoas"]);

    private static void AssertNoDependency(string layer, string[] forbiddenRoots, params string[] forbiddenLayerSuffixes)
    {
        var violations = Solution.Layer(layer)
            .SelectMany(assembly => Solution.ReferencesOf(assembly)
                .Where(reference => forbiddenRoots.Any(root => Solution.IsUnder(reference, root))
                    || (Solution.ModuleOf(reference) is not null && forbiddenLayerSuffixes.Any(suffix => reference.EndsWith(suffix, StringComparison.Ordinal))))
                .Select(reference => $"{Solution.NameOf(assembly)} -> {reference}"))
            .ToList();

        Assert.True(Solution.Layer(layer).Any(), $"No {layer} assemblies were found next to the tests.");
        Assert.True(violations.Count == 0, $"{layer} has forbidden dependencies:\n{string.Join("\n", violations)}");
    }
}
