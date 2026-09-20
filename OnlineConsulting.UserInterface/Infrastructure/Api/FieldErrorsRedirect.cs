using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>
/// Carries a failed <see cref="ApiEnvelope"/>'s FieldErrors across a POST-redirect-GET so they land
/// under the right input via asp-validation-for, without every controller wiring ModelState by hand.
/// Pair: call <see cref="RedirectToActionWithFieldErrors"/> from the POST action instead of a plain
/// RedirectToAction; <see cref="FieldErrorsModelStateFilter"/> applies them to ModelState automatically
/// before the destination GET action runs.
/// </summary>
public static class FieldErrorsRedirect
{
    internal const string TempDataKey = "FieldErrorsRedirect.FieldErrors";

    /// <summary>
    /// Redirects like <see cref="Controller.RedirectToAction(string, string, object)"/>, but if
    /// <paramref name="envelope"/> failed with FieldErrors, stashes them (prefixed with
    /// <paramref name="modelPrefix"/>, e.g. "ChangePassword" for a "ChangePassword.NewPassword" field)
    /// so the next request's ModelState carries them for the view's validation spans to render.
    /// </summary>
    public static IActionResult RedirectToActionWithFieldErrors(
        this Controller controller,
        ApiEnvelope envelope,
        string modelPrefix,
        string actionName,
        string? controllerName = null,
        object? routeValues = null)
    {
        if (envelope.FieldErrors is { Count: > 0 })
        {
            var prefixed = envelope.FieldErrors.ToDictionary(pair => $"{modelPrefix}.{pair.Key}", pair => pair.Value);
            controller.TempData[TempDataKey] = JsonSerializer.Serialize(prefixed);
        }

        return controllerName is null
            ? controller.RedirectToAction(actionName, routeValues)
            : controller.RedirectToAction(actionName, controllerName, routeValues);
    }
}
