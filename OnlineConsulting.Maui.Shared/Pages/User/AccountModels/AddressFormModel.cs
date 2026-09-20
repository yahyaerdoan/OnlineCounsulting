namespace OnlineConsulting.Maui.Shared.Pages.User.AccountModels;

public class AddressFormModel
{
    public string AddressName { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string Country { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public bool IsShippingAddress { get; set; }

    public bool IsBillingAddress { get; set; }
}
