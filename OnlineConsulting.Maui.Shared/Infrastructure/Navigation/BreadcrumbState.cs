namespace OnlineConsulting.Maui.Shared.Infrastructure.Navigation;

/// <summary>Lets a routed sub-page (e.g. a create/edit form) publish the trailing breadcrumb
/// segment its own route can't derive from NavSections - mirrors Blazor's PageTitle pattern.</summary>
public class BreadcrumbState
{
    public string? ExtraSegment { get; private set; }

    public event Action? Changed;

    public void Set(string? text)
    {
        ExtraSegment = text;
        Changed?.Invoke();
    }
}
