namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Lets MainLayout pick drawer style per host: Web (false) gets a resizable icon-rail, MAUI (true) gets a full overlay - a rail resize was invisible enough on a phone to look broken.</summary>
public interface IPlatformInfo
{
    bool IsNativeMobile { get; }
}
