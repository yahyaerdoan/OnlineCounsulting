using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Components.Common;

/// <summary>One list-row action for <see cref="RowActions"/>; set Href to navigate, OnClick to run, or Items for a choice list.</summary>
public sealed record RowAction(string Text, string Icon)
{
    public string? Href { get; init; }

    public Func<Task>? OnClick { get; init; }

    /// <summary>Choices shown in a dropdown, e.g. payment methods; Icon is unused on items.</summary>
    public IReadOnlyList<RowAction>? Items { get; init; }

    public bool Disabled { get; init; }

    /// <summary>False leaves the action out entirely, for state-dependent actions.</summary>
    public bool Visible { get; init; } = true;

    public Color Color { get; init; } = Color.Default;
}
