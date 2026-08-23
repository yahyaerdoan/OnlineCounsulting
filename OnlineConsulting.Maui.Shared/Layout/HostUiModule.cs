using System.Reflection;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Registers a host's own assembly for @page discovery (its pages, e.g. Login).</summary>
public class HostUiModule(Assembly hostAssembly) : IUiModule
{
    public Assembly? Assembly { get; } = hostAssembly;
}
