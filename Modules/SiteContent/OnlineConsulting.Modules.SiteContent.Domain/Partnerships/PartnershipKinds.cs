namespace OnlineConsulting.Modules.SiteContent.Domain.Partnerships;

/// <summary>Partnership.Kind: which About page section the card appears in.</summary>
public static class PartnershipKinds
{
    public const string Partner = "Partner";
    public const string Team = "Team";

    public static readonly string[] All = [Partner, Team];
}
