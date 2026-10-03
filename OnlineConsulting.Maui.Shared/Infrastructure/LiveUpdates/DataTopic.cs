namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>What changed, so only the screens showing that data reload. Names match the Api's DataChangeTopics strings.</summary>
public enum DataTopic
{
    /// <summary>Everything the user might be looking at - app resumed, pull-to-refresh, live connection re-established.</summary>
    All,

    /// <summary>Small always-visible counters (cart badge, dashboard header) - refreshed on navigation.</summary>
    Summary,

    Basket,
    Orders,
    Appointments,
    Membership,
    Profile,
    Addresses,
    Equipment,
    Referrals,
    Notifications,
}
