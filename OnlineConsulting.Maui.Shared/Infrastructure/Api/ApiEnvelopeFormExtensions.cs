using Microsoft.AspNetCore.Components.Forms;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Maps an unsuccessful ApiEnvelope onto an EditForm's validation state.</summary>
public static class ApiEnvelopeFormExtensions
{
    /// <summary>Returns the general error, or null if it wrote field errors instead.</summary>
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
