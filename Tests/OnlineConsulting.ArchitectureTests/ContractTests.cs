using System.Reflection;

namespace OnlineConsulting.ArchitectureTests;

public class ContractTests
{
    private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [Fact]
    public void Contracts_expose_no_domain_types()
    {
        var violations = Solution.Layer("Application")
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => type.Namespace?.EndsWith(".Contracts", StringComparison.Ordinal) == true)
            .SelectMany(type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => TypesIn(property.PropertyType).Any(IsDomainType))
                .Select(property => $"{type.FullName}.{property.Name}: {property.PropertyType.Name}"))
            .ToList();

        Assert.True(violations.Count == 0, $"Contracts leak domain types onto the wire; map them to primitives or contract types:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void Application_types_carry_no_json_attributes()
    {
        var violations = Solution.Layer("Application")
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => new MemberInfo[] { type }.Concat(type.GetMembers(Members))
                .Where(member => member.GetCustomAttributesData().Any(attribute => attribute.AttributeType.Namespace == "System.Text.Json.Serialization"))
                .Select(member => member == type ? type.FullName : $"{type.FullName}.{member.Name}"))
            .ToList();

        Assert.True(violations.Count == 0, $"JSON shape is the Api's concern (request DTOs, contracts as plain records):\n{string.Join("\n", violations)}");
    }

    private static bool IsDomainType(Type type) =>
        Solution.ModuleOf(Solution.NameOf(type.Assembly)) is not null && Solution.NameOf(type.Assembly).EndsWith(".Domain", StringComparison.Ordinal);

    private static IEnumerable<Type> TypesIn(Type type)
    {
        yield return type;

        var nested = type.IsArray ? [type.GetElementType() ?? type] : type.IsGenericType ? type.GetGenericArguments() : Array.Empty<Type>();
        foreach (var inner in nested.Where(inner => inner != type).SelectMany(TypesIn))
        {
            yield return inner;
        }
    }
}
