using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Forms;

/// <summary>Shared EditForm state (busy flag + validation store). Inject as "Form", call Bind(model) in OnInitialized.</summary>
public sealed class FormState<TModel> where TModel : class
{
    private readonly BusySubmit _busy = new();

    public bool IsBusy => _busy.IsBusy;

    public EditContext EditContext
    {
        get => field ?? throw NotBound();
        private set;
    }

    private ValidationMessageStore ValidationMessages
    {
        get => field ?? throw NotBound();
        set;
    }

    /// <summary>Call once in OnInitialized (not the constructor - form-bound models aren't set yet there).</summary>
    public void Bind(TModel model)
    {
        EditContext = new EditContext(model);
        ValidationMessages = new ValidationMessageStore(EditContext);

        // Validate() fails forever on stale field errors otherwise - nothing here revalidates them.
        EditContext.OnValidationRequested += (_, _) => ValidationMessages.Clear();
    }

    /// <summary>Wraps a submit handler, tracking IsBusy.</summary>
    public Task SubmitAsync(Func<Task> submit) => _busy.RunAsync(submit);

    /// <summary>On a failed API result: writes field errors onto the form, returns the leftover general error (or null).</summary>
    public string? DisplayErrors(IApiResult result) => result.DisplayErrors(EditContext, ValidationMessages);

    /// <summary>Same, but also toasts the general error via Snackbar. Only for pages with a live circuit - not static SSR.</summary>
    public void DisplayErrors(IApiResult result, ISnackbar snackbar)
    {
        var generalError = DisplayErrors(result);
        if (generalError is not null)
        {
            snackbar.ShowError(generalError);
        }
    }

    private static InvalidOperationException NotBound() =>
        new($"FormState<{typeof(TModel).Name}> not bound - call Bind(model) in OnInitialized first.");
}
