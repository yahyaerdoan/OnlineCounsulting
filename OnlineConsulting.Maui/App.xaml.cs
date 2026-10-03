using OnlineConsulting.Maui.Infrastructure;

namespace OnlineConsulting.Maui;

public partial class App : Application
{
    private readonly MauiAppResumeSource _resumeSource;

    public App(MauiAppResumeSource resumeSource)
    {
        InitializeComponent();
        _resumeSource = resumeSource;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "ComfortPro" };
        window.Resumed += (_, _) => _resumeSource.RaiseResumed();
        return window;
    }
}
