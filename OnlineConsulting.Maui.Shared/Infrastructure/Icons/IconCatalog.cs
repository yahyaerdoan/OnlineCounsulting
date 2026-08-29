using MudBlazor;
using System.Reflection;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Icons;

/// <summary>Searchable, cached index over MudBlazor's ~2000 icon constants.</summary>
public static class IconCatalog
{
    private static readonly Lazy<List<(string Name, string Value)>> All = new(BuildCatalog);

    // Several icon names share one SVG (aliases) and some names only differ by case - ToDictionary
    // throws on the first duplicate key, so build these by hand and keep the first match instead.
    private static readonly Lazy<Dictionary<string, string>> NamesByValue = new(() => IndexBy(icon => icon.Value, icon => icon.Name, StringComparer.Ordinal));
    private static readonly Lazy<Dictionary<string, string>> ValuesByName = new(() => IndexBy(icon => icon.Name, icon => icon.Value, StringComparer.OrdinalIgnoreCase));

    // Search results must have unique values - MudAutocomplete keys its list items by T, and aliases
    // sharing one SVG would render two list items with the same key otherwise.
    private static readonly Lazy<List<(string Name, string Value)>> Distinct = new(() =>
        [.. NamesByValue.Value.Select(pair => (Name: pair.Value, Value: pair.Key)).OrderBy(icon => icon.Name, StringComparer.Ordinal)]);

    private static Dictionary<string, string> IndexBy(Func<(string Name, string Value), string> key, Func<(string Name, string Value), string> value, IEqualityComparer<string> comparer)
    {
        var map = new Dictionary<string, string>(comparer);
        foreach (var icon in All.Value)
        {
            _ = map.TryAdd(key(icon), value(icon));
        }

        return map;
    }

    public static IReadOnlyList<string> Search(string? term, int maxResults = 50)
    {
        var matches = string.IsNullOrWhiteSpace(term)
            ? Distinct.Value.AsEnumerable()
            : Distinct.Value.Where(icon => icon.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

        return [.. matches.Take(maxResults).Select(icon => icon.Value)];
    }

    /// <summary>Display name for a Search result - falls back to the raw value if unknown.</summary>
    public static string NameOf(string? value) =>
        value is not null && NamesByValue.Value.TryGetValue(value, out var name) ? name : value ?? string.Empty;

    /// <summary>Normalizes a stored Icon value for rendering - passes known SVG values through,
    /// resolves a legacy dotted name (e.g. "Icons.Material.Filled.Hvac") to its real value, else
    /// returns the input unchanged.</summary>
    public static string Resolve(string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
        {
            return string.Empty;
        }

        if (NamesByValue.Value.ContainsKey(storedValue))
        {
            return storedValue;
        }

        var name = storedValue[(storedValue.LastIndexOf('.') + 1)..];
        return ValuesByName.Value.TryGetValue(name, out var value) ? value : storedValue;
    }

    private static List<(string Name, string Value)> BuildCatalog()
    {
        var result = new List<(string Name, string Value)>();

        foreach (var field in typeof(global::MudBlazor.Icons.Material.Outlined).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is string value)
            {
                result.Add((field.Name, value));
            }
        }

        result.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
        return result;
    }
}
