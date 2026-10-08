using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OnlineConsulting.Api.Tests.Contracts;

public static class WireSchema
{
    private static readonly Dictionary<Type, string> Keywords = new()
    {
        [typeof(string)] = "string",
        [typeof(bool)] = "bool",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(object)] = "object",
    };

    /// <summary>The JSON members of a type in the order they are written, as "name: type" lines.</summary>
    public static IReadOnlyList<string> Describe(JsonSerializerOptions options, Type type)
    {
        var nullability = new NullabilityInfoContext();
        return [.. options.GetTypeInfo(type).Properties
            .OrderBy(property => property.Order)
            .Select(property => $"{property.Name}: {Describe(property, nullability)}")];
    }

    private static string Describe(JsonPropertyInfo property, NullabilityInfoContext nullability) =>
        property.AttributeProvider is PropertyInfo member
            ? Describe(nullability.Create(member))
            : Name(property.PropertyType) + "?";

    private static string Describe(NullabilityInfo info)
    {
        if (Nullable.GetUnderlyingType(info.Type) is { } underlying)
        {
            return Name(underlying) + "?";
        }

        var name = info.ElementType is { } element
            ? Describe(element) + "[]"
            : info.Type.IsGenericType
                ? $"{GenericName(info.Type)}<{string.Join(", ", info.GenericTypeArguments.Select(Describe))}>"
                : Name(info.Type);
        return info.ReadState == NullabilityState.Nullable ? name + "?" : name;
    }

    private static string Name(Type type) => Keywords.TryGetValue(type, out var keyword) ? keyword : type.IsGenericType ? GenericName(type) : type.Name;

    private static string GenericName(Type type) => type.Name[..type.Name.IndexOf('`')];
}
