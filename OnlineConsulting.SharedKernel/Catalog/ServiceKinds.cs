namespace OnlineConsulting.SharedKernel.Catalog;

/// <summary>How a catalog service is sold: Booking goes through the appointment flow (a visit is scheduled), Product goes
/// through basket and checkout (bought and paid online).</summary>
public static class ServiceKinds
{
    public const string Booking = "Booking";
    public const string Product = "Product";

    public static readonly string[] All = [Booking, Product];
}
