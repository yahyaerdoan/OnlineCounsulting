using NToastNotify;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using ResultHandler.Core.Enums;
using ResultHandler.Mapping;
namespace OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

public static class NToastService
{
    /// <summary>Shows a success/failure toast for an <see cref="ApiEnvelope"/> result.</summary>
    public static void ShowResult(this IToastNotification toastNotification, ApiEnvelope result) =>
        Show(toastNotification, result.DisplayMessage, result.IsSuccessful ? ResultStatus.Ok : ResultStatus.BadRequest);

    /// <summary>Shows a success/failure toast for an <see cref="ApiEnvelope{T}"/> result.</summary>
    public static void ShowResult<T>(this IToastNotification toastNotification, ApiEnvelope<T> result) =>
        Show(toastNotification, result.DisplayMessage, result.IsSuccessful ? ResultStatus.Ok : ResultStatus.BadRequest);

    /// <summary>Shows a toast whose color/icon is derived from <paramref name="status"/>.</summary>
    public static void Show(IToastNotification toastNotification, string message, ResultStatus status, string? title = null)
    {
        var (type, defaultTitle) = Describe(status);

        var options = new ToastrOptions { Title = title ?? defaultTitle, CloseButton = true, TimeOut = 5000, PositionClass = ToastPositions.TopRight };

        switch (type)
        {
            case ToastType.Success:
                toastNotification.AddSuccessToastMessage(message, options);
                break;
            case ToastType.Info:
                toastNotification.AddInfoToastMessage(message, options);
                break;
            case ToastType.Warning:
                toastNotification.AddWarningToastMessage(message, options);
                break;
            case ToastType.Error:
                toastNotification.AddErrorToastMessage(message, options);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unhandled ToastType.");
        }
    }

    private enum ToastType { Success, Info, Warning, Error }

    private static readonly Dictionary<ResultStatus, (ToastType Type, string Title)> _overrides = new()
    {
        [ResultStatus.Ok] = (ToastType.Success, "Successful!"),
        [ResultStatus.Created] = (ToastType.Success, "Created!"),
        [ResultStatus.Unauthorized] = (ToastType.Warning, "Unauthorized!"),
        [ResultStatus.NotFound] = (ToastType.Error, "Not Found!"),
        [ResultStatus.BadRequest] = (ToastType.Error, "Bad Request!"),
        [ResultStatus.Forbidden] = (ToastType.Error, "Forbidden!"),
        [ResultStatus.InternalServerError] = (ToastType.Error, "Internal Server Error!"),
    };

    private static (ToastType Type, string Title) Describe(ResultStatus status)
        => _overrides.TryGetValue(status, out var overridden) ? overridden : DescribeByHttpClass(status);
    private static (ToastType Type, string Title) DescribeByHttpClass(ResultStatus status) => (int)status.ToHttpStatusCode() switch
    {
        >= 200 and < 300 => (ToastType.Success, "Successful!"),
        >= 300 and < 400 => (ToastType.Info, "Redirect!"),
        >= 400 and < 500 => (ToastType.Warning, status.ToString()),
        >= 500 => (ToastType.Error, status.ToString()),
        _ => (ToastType.Info, "Info!")
    };
}
