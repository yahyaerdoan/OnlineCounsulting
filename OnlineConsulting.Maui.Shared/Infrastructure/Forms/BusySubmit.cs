namespace OnlineConsulting.Maui.Shared.Infrastructure.Forms;

/// <summary>Toggles IsBusy around an async submit so no branch can forget to reset it.
/// No try/finally on purpose - unhandled exceptions should hit the app's central error boundary, not be masked here.</summary>
public sealed class BusySubmit
{
    public bool IsBusy { get; private set; }

    public async Task RunAsync(Func<Task> submit)
    {
        IsBusy = true;
        await submit();
        IsBusy = false;
    }
}
