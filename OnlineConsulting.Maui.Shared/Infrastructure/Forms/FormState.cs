using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Forms;

/// <summary>Shared EditForm state: busy flag + validation store, one line per form.
/// Usage: inject FormState of your model type as "Form"; call Form.Bind(model) in OnInitialized;
/// wrap submit in Form.SubmitAsync(...); on failure call Form.DisplayErrors(result[, Snackbar]).
/// Registered Transient - each component gets its own instance.</summary>
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
    }

    /// <summary>Wraps a submit handler: clears old field errors, tracks IsBusy.</summary>
    public Task SubmitAsync(Func<Task> submit)
    {
        ValidationMessages.Clear();
        return _busy.RunAsync(submit);
    }

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
