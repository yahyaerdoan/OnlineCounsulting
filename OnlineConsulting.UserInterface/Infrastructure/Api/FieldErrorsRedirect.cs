using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Carries a failed <see cref="ApiEnvelope"/>'s FieldErrors across a POST-redirect-GET; paired with <see cref="FieldErrorsModelStateFilter"/> to apply them before the destination GET runs.</summary>
public static class FieldErrorsRedirect
{
    internal const string TempDataKey = "FieldErrorsRedirect.FieldErrors";

    /// <summary>Like RedirectToAction, but also stashes FieldErrors prefixed with <paramref name="modelPrefix"/> for the next request's ModelState.</summary>
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
