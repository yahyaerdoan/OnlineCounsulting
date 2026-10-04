using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Unread count behind every bell icon on the page; reloads itself when the inbox changes (live signal, app
/// resume, navigation). Only calls the Api while signed in - an anonymous 401 would trigger the session-expired sign-out.</summary>
public sealed class NotificationState : IDisposable
{
    private readonly IApiClient _apiClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IDisposable _subscription;
    private Task? _loadTask;

    public NotificationState(IApiClient apiClient, AuthenticationStateProvider authStateProvider, DataChangeNotifier notifier)
    {
        _apiClient = apiClient;
        _authStateProvider = authStateProvider;
        _subscription = notifier.Subscribe(ReloadAsync, DataTopic.Notifications, DataTopic.Summary);
    }

    public int UnreadCount { get; private set; }

    public event Action? Changed;

    /// <summary>Loads once per circuit/app session - safe to call from every bell instance.</summary>
    public Task EnsureLoadedAsync() => _loadTask ??= ReloadAsync();

    public async Task ReloadAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated is not true)
        {
            SetCount(0);
            return;
        }

        var result = await _apiClient.GetAsync<int>(ApiRoutes.Notifications.UnreadCount);
        if (result.IsSuccessful)
        {
            SetCount(result.ResultData);
        }
    }

    /// <summary>Optimistic local update after the user reads something; the live signal confirms it moments later.</summary>
    public void SetCount(int count)
    {
        UnreadCount = Math.Max(0, count);
        Changed?.Invoke();
    }

    /// <summary>Where tapping a notification should go, from its payload - staff land on the admin screens, customers on theirs.</summary>
    public static string? LinkFor(UserNotificationResponse notification, ClaimsPrincipal user)
    {
        var isCustomer = user.IsInRole(AppRoles.User) && user.FindAll(ClaimTypes.Role).Count() == 1;
        var data = notification.Data;
        return true switch
        {
            _ when data.TryGetValue("invoiceId", out var invoiceId) => isCustomer ? $"/user/invoices/{invoiceId}" : "/admin/commerce/invoices",
            _ when data.TryGetValue("orderId", out var orderId) => isCustomer ? $"/user/orders/{orderId}" : "/admin/commerce/orders",
            _ when data.ContainsKey("appointmentId") => isCustomer ? "/user/appointments" : "/admin/operations/appointments",
            _ when data.ContainsKey("customerMembershipId") => "/user/membership",
            _ when data.ContainsKey("referralId") => "/user/referrals",
            _ => null,
        };
    }

    public void Dispose() => _subscription.Dispose();
}
