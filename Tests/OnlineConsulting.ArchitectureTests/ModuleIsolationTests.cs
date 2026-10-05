namespace OnlineConsulting.ArchitectureTests;

public class ModuleIsolationTests
{
    [Fact]
    public void Modules_talk_to_each_other_only_through_the_shared_kernel()
    {
        var violations = Solution.ModuleAssemblies
            .SelectMany(assembly => Solution.ReferencesOf(assembly)
                .Where(reference => Solution.ModuleOf(reference) is { } other && other != Solution.ModuleOf(Solution.NameOf(assembly)))
                .Select(reference => $"{Solution.NameOf(assembly)} -> {reference}"))
            .ToList();

        Assert.True(Solution.ModuleAssemblies.Count > 0, "No module assemblies were found next to the tests.");
        Assert.True(violations.Count == 0, $"Modules reference each other; go through an OnlineConsulting.SharedKernel contract instead:\n{string.Join("\n", violations)}");
    }
}
