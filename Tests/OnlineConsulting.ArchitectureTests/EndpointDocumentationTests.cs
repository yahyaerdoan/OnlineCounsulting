using System.Text.RegularExpressions;

namespace OnlineConsulting.ArchitectureTests;

public partial class EndpointDocumentationTests
{
    [Fact]
    public void Every_endpoint_documents_its_success_response()
    {
        var api = Path.Combine(Solution.RootDirectory(), "OnlineConsulting.Api");

        var violations = Directory.EnumerateFiles(api, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Split(Path.DirectorySeparatorChar).Any(segment => segment is "obj" or "bin"))
            .Where(path => MapCall().IsMatch(File.ReadAllText(path)))
            .Where(path => !SuccessResponse().IsMatch(File.ReadAllText(path)))
            .Select(path => Path.GetRelativePath(Solution.RootDirectory(), path))
            .ToList();

        Assert.True(violations.Count == 0,
            $"Declare the success response for OpenAPI and generated clients: .ProducesEnveloped<T>() for an enveloped result, .ProducesEnveloped() without data, .Produces(status) otherwise:\n{string.Join("\n", violations)}");
    }

    [GeneratedRegex(@"\.Map(Get|Post|Put|Delete|Patch)\(")]
    private static partial Regex MapCall();

    [GeneratedRegex(@"\.(ProducesEnveloped|Produces)\b")]
    private static partial Regex SuccessResponse();
}
