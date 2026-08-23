using System.Reflection;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Combines every IUiModule's assemblies and nav sections for Routes.razor/MainLayout.
/// Resolved via [Inject], not [Parameter] - Assembly isn't JSON-serializable.</summary>
public class UiModuleRegistry(IEnumerable<IUiModule> modules)
{
    public IReadOnlyList<Assembly> AdditionalAssemblies { get; } =
        modules.Select(m => m.Assembly).OfType<Assembly>().Distinct().ToList();

    public IReadOnlyList<NavSection> NavSections { get; } =
        modules.SelectMany(m => m.NavSections).ToList();
}
