namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>The US street address fields UsAddressFields edits in place - implemented by each form model, so validation messages land on that form's own fields.</summary>
public interface IUsAddress
{
    string AddressLine { get; set; }

    string City { get; set; }

    string State { get; set; }

    string Zipcode { get; set; }
}

public static class UsStates
{
    public static readonly (string Code, string Name)[] All =
    [
        ("AL", "Alabama"), ("AK", "Alaska"), ("AZ", "Arizona"), ("AR", "Arkansas"), ("CA", "California"), ("CO", "Colorado"),
        ("CT", "Connecticut"), ("DE", "Delaware"), ("DC", "District of Columbia"), ("FL", "Florida"), ("GA", "Georgia"), ("HI", "Hawaii"),
        ("ID", "Idaho"), ("IL", "Illinois"), ("IN", "Indiana"), ("IA", "Iowa"), ("KS", "Kansas"), ("KY", "Kentucky"), ("LA", "Louisiana"),
        ("ME", "Maine"), ("MD", "Maryland"), ("MA", "Massachusetts"), ("MI", "Michigan"), ("MN", "Minnesota"), ("MS", "Mississippi"),
        ("MO", "Missouri"), ("MT", "Montana"), ("NE", "Nebraska"), ("NV", "Nevada"), ("NH", "New Hampshire"), ("NJ", "New Jersey"),
        ("NM", "New Mexico"), ("NY", "New York"), ("NC", "North Carolina"), ("ND", "North Dakota"), ("OH", "Ohio"), ("OK", "Oklahoma"),
        ("OR", "Oregon"), ("PA", "Pennsylvania"), ("RI", "Rhode Island"), ("SC", "South Carolina"), ("SD", "South Dakota"),
        ("TN", "Tennessee"), ("TX", "Texas"), ("UT", "Utah"), ("VT", "Vermont"), ("VA", "Virginia"), ("WA", "Washington"),
        ("WV", "West Virginia"), ("WI", "Wisconsin"), ("WY", "Wyoming"),
    ];

    /// <summary>"123 Main St, Austin, TX 78701" - the one-line form an appointment stores and a maps link opens.</summary>
    public static string OneLine(IUsAddress address) =>
        $"{address.AddressLine.Trim()}, {address.City.Trim()}, {address.State.Trim()} {address.Zipcode.Trim()}".Trim();
}
