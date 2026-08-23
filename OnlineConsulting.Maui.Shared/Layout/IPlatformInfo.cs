namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Lets MainLayout pick a drawer style appropriate to the host: Web registers false (a
/// browser tab can be resized, but defaults to desktop-width, where a persistent icon-rail makes
/// sense); the MAUI head registers true (always phone/tablet-sized, where the drawer should be a
/// full overlay that opens/closes instead of a rail that just changes width - the rail resize was
/// invisible enough on a narrow screen to look like the toggle button did nothing).</summary>
public interface IPlatformInfo
{
    bool IsNativeMobile { get; }
}
