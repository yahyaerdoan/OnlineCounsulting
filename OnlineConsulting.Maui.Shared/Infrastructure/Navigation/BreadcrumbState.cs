namespace OnlineConsulting.Maui.Shared.Infrastructure.Navigation;

/// <summary>Lets a sub-page publish its trailing breadcrumb segment - mirrors Blazor's PageTitle.</summary>
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
