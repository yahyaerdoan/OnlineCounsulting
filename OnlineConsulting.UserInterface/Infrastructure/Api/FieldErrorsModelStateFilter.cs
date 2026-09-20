using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>
/// Applies FieldErrors stashed by <see cref="FieldErrorsRedirect.RedirectToActionWithFieldErrors"/> to
/// the current action's ModelState, before it runs - registered once globally so no controller needs
/// to read TempData itself. <see cref="ITempDataDictionary.Peek"/>, not indexing, so the entry survives
/// for the view (asp-validation-for reads ModelState, but keeping TempData intact avoids surprises if
/// something else also reads this key).
/// </summary>
public class FieldErrorsModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not Controller controller)
        {
            return;
        }

        if (controller.TempData.Peek(FieldErrorsRedirect.TempDataKey) is not string json)
        {
            return;
        }

        var fieldErrors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
        if (fieldErrors is null)
        {
            return;
        }

        foreach (var (field, messages) in fieldErrors)
        {
            foreach (var message in messages)
            {
                controller.ModelState.AddModelError(field, message);
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
