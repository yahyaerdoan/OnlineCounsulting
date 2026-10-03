namespace OnlineConsulting.Maui.Shared.Components.Common;

/// <summary>One tab of a ListFilterTabs row; Key is what the page sends to the API (empty for "All").</summary>
public sealed record ListFilterOption(string Key, string Label, string? Icon = null);
