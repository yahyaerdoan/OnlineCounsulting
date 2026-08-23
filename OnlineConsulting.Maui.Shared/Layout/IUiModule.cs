using System.Reflection;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Plug-in seam for a feature module - register one, its pages/nav appear automatically.</summary>
public interface IUiModule
{
    Assembly? Assembly => null;

    IReadOnlyList<NavSection> NavSections => [];
}
