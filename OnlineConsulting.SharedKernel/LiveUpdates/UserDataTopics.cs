namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Topic names sent to clients in a "data changed" signal; they match the storefront's DataTopic enum names.</summary>
public static class UserDataTopics
{
    public const string Basket = "Basket";
    public const string Orders = "Orders";
    public const string Appointments = "Appointments";
    public const string Membership = "Membership";
    public const string Profile = "Profile";
    public const string Addresses = "Addresses";
    public const string Equipment = "Equipment";
    public const string Referrals = "Referrals";
    public const string Notifications = "Notifications";
}
