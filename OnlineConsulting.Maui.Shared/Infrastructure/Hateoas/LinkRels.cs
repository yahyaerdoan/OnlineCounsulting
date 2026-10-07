namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>Link relations the Api sends, as they appear in "_links" (custom ones carry the "oc:" CURIE prefix, documented at /rels).</summary>
public static class LinkRels
{
    public const string Self = "self";
    public const string Edit = "edit";
    public const string Pay = "oc:pay";
    public const string Cancel = "oc:cancel";
    public const string ChangeAddresses = "oc:change-addresses";
    public const string Refund = "oc:refund";
    public const string MarkPaid = "oc:mark-paid";
    public const string Void = "oc:void";
    public const string Pdf = "oc:pdf";
    public const string Confirm = "oc:confirm";
    public const string AssignTechnician = "oc:assign-technician";
    public const string WorkOrder = "oc:work-order";
    public const string RecordWorkOrder = "oc:record-work-order";
    public const string Pause = "oc:pause";
    public const string Resume = "oc:resume";
    public const string Reactivate = "oc:reactivate";
    public const string ChangePlan = "oc:change-plan";
    public const string SetActive = "oc:set-active";
    public const string Suspend = "oc:suspend";
    public const string ChangeTimeZone = "oc:change-time-zone";
    public const string UpdateBranding = "oc:update-branding";
    public const string Delete = "oc:delete";
    public const string Roles = "oc:roles";
    public const string AssignRoles = "oc:assign-roles";
    public const string PermissionOverrides = "oc:permission-overrides";
    public const string MarkRead = "oc:mark-read";
    public const string Reply = "oc:reply";
}
