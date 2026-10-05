namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>Link relations the Api sends, as they appear in "_links" (custom ones carry the "oc:" CURIE prefix, documented at /rels).</summary>
public static class LinkRels
{
    public const string Self = "self";
    public const string Pay = "oc:pay";
    public const string Cancel = "oc:cancel";
    public const string ChangeAddresses = "oc:change-addresses";
    public const string Refund = "oc:refund";
    public const string MarkPaid = "oc:mark-paid";
    public const string Void = "oc:void";
    public const string Pdf = "oc:pdf";
}
