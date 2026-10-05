using System.Reflection;
using System.Runtime.CompilerServices;

namespace OnlineConsulting.ArchitectureTests;

public static class Solution
{
    private const string ModulePrefix = "OnlineConsulting.Modules.";

    private static readonly Lazy<IReadOnlyList<Assembly>> LoadedModuleAssemblies = new(() =>
        [.. Directory.GetFiles(AppContext.BaseDirectory, ModulePrefix + "*.dll")
            .Where(path => !Path.GetFileName(path).Contains(".Tests", StringComparison.Ordinal))
            .Select(path => Assembly.LoadFrom(path))
            .OrderBy(assembly => assembly.GetName().Name, StringComparer.Ordinal)]);

    public static IReadOnlyList<Assembly> ModuleAssemblies => LoadedModuleAssemblies.Value;

    /// <summary>Module assemblies of one layer ("Domain", "Application" or "Infrastructure").</summary>
    public static IEnumerable<Assembly> Layer(string layer) => ModuleAssemblies.Where(assembly => NameOf(assembly).EndsWith("." + layer, StringComparison.Ordinal));

    /// <summary>The module an assembly belongs to ("Commerce" for OnlineConsulting.Modules.Commerce.Domain); null outside the modules.</summary>
    public static string? ModuleOf(string assemblyName) =>
        assemblyName.StartsWith(ModulePrefix, StringComparison.Ordinal) ? assemblyName[ModulePrefix.Length..].Split('.')[0] : null;

    public static string NameOf(Assembly assembly) => assembly.GetName().Name ?? throw new InvalidOperationException("An assembly without a name.");

    /// <summary>The assemblies an assembly actually uses; the compiler drops references nothing in it touches.</summary>
    public static IEnumerable<string> ReferencesOf(Assembly assembly) => assembly.GetReferencedAssemblies().Select(reference => reference.Name ?? string.Empty);

    /// <summary>True for the named assembly or any assembly under it (Microsoft.AspNetCore matches Microsoft.AspNetCore.Http).</summary>
    public static bool IsUnder(string assemblyName, string root) =>
        assemblyName == root || assemblyName.StartsWith(root + ".", StringComparison.Ordinal);

    public static string RootDirectory([CallerFilePath] string sourceFile = "") =>
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFile) ?? throw new InvalidOperationException("No source path."), "..", ".."));
}
