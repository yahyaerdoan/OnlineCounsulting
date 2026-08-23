using Microsoft.AspNetCore.Components.Forms;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Maps an unsuccessful ApiEnvelope onto an EditForm. General-error display (Snackbar, inline, ...) is left to the caller.</summary>
public static class ApiEnvelopeFormExtensions
{
    /// <summary>Field errors go into the validation store (returns null). No field errors -> returns the general error message to display yourself.</summary>
    public static string? DisplayErrors(this IApiResult result, EditContext editContext, ValidationMessageStore validationMessages)
    {
        validationMessages.Clear();

        if (result.FieldErrors is not { Count: > 0 })
        {
            return result.DisplayMessage;
        }

        foreach (var (property, messages) in result.FieldErrors)
        {
            validationMessages.Add(editContext.Field(property), messages);
        }

        editContext.NotifyValidationStateChanged();
        return null;
    }
}
