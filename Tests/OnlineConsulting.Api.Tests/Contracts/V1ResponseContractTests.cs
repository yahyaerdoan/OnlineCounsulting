using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Common.Hateoas;

namespace OnlineConsulting.Api.Tests.Contracts;

public class V1ResponseContractTests
{
    private static readonly Assembly ApiAssembly = typeof(Rels).Assembly;

    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static TheoryData<string> Modules() => [.. ContractsByModule().Keys];

    [Theory]
    [MemberData(nameof(Modules))]
    public void Response_shapes_match_the_golden_file(string module)
    {
        var options = ApiJsonOptions();
        var shapes = ContractsByModule()[module].ToDictionary(type => type.Name, type => WireSchema.Describe(options, type));
        var actual = JsonSerializer.Serialize(shapes, WriteOptions).ReplaceLineEndings("\n") + "\n";

        var golden = Path.Combine(GoldenDirectory(), module + ".json");
        var received = Path.ChangeExtension(golden, ".received.json");
        if (File.Exists(golden) && File.ReadAllText(golden).ReplaceLineEndings("\n") == actual)
        {
            File.Delete(received);
            return;
        }

        File.WriteAllText(received, actual);
        Assert.Fail($"The v1 wire shape of {module} changed. Clients depend on it: put a breaking change in a new API version, and replace {Path.GetFileName(golden)} with {Path.GetFileName(received)} only for an additive change.");
    }

    private static JsonSerializerOptions ApiJsonOptions()
    {
        var services = new ServiceCollection();
        _ = services.AddLogging().AddRouting().AddApiJson();
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value.SerializerOptions;
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }

    private static SortedDictionary<string, List<Type>> ContractsByModule()
    {
        var modules = new SortedDictionary<string, List<Type>>(StringComparer.Ordinal);
        foreach (var assembly in ApiAssembly.GetReferencedAssemblies().Where(name => name.Name?.EndsWith(".Application", StringComparison.Ordinal) == true).Select(Assembly.Load))
        {
            var module = (assembly.GetName().Name ?? throw new InvalidOperationException("An assembly without a name.")).Split('.')[^2];
            modules[module] = [.. assembly.GetExportedTypes().Where(type => type.Namespace?.EndsWith(".Contracts", StringComparison.Ordinal) == true && IsResponse(type)).OrderBy(type => type.Name, StringComparer.Ordinal)];
        }

        modules["Api"] = [.. ApiAssembly.GetExportedTypes().Where(type => type.Name.EndsWith("Response", StringComparison.Ordinal) && IsResponse(type)).OrderBy(type => type.Name, StringComparer.Ordinal)];
        return modules;
    }

    private static bool IsResponse(Type type) =>
        type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false } && !type.Name.EndsWith("Request", StringComparison.Ordinal);

    private static string GoldenDirectory([CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile) ?? throw new InvalidOperationException("No source path."), "Golden", "v1");
}
