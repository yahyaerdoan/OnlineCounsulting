namespace OnlineConsulting.Maui.Shared.Components.Common;

/// <summary>Which users a <see cref="UserPicker"/> offers.</summary>
public enum UserPickerAudience
{
    All,

    /// <summary>The team (any role besides User), e.g. technicians.</summary>
    Staff,

    /// <summary>Storefront users, e.g. an equipment owner.</summary>
    Customers,
}
