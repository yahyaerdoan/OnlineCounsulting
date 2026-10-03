namespace OnlineConsulting.Api.Common.Hateoas;

/// <summary>
/// Every application (extension) link relation the API emits, written as "oc:{name}" in "_links", with the description served at
/// /rels/{name}. IANA relations (self, edit, ...) come from Hateoas.LinkRelations. Add a relation here, with its description, before
/// using it in a link provider.
/// </summary>
public static class Rels
{
    public const string CurieName = "oc";

    public const string Delete = "delete";
    public const string Cancel = "cancel";
    public const string Pay = "pay";
    public const string Refund = "refund";
    public const string Void = "void";
    public const string MarkPaid = "mark-paid";
    public const string Pdf = "pdf";
    public const string Confirm = "confirm";
    public const string AssignTechnician = "assign-technician";
    public const string WorkOrder = "work-order";
    public const string WorkOrders = "work-orders";
    public const string Appointment = "appointment";
    public const string AddMedia = "add-media";
    public const string Media = "media";
    public const string Pause = "pause";
    public const string Resume = "resume";
    public const string Reactivate = "reactivate";
    public const string ChangePlan = "change-plan";
    public const string Plan = "plan";
    public const string Subscribe = "subscribe";
    public const string SetActive = "set-active";
    public const string SetShipping = "set-shipping";
    public const string SetBilling = "set-billing";
    public const string ChangeAddresses = "change-addresses";
    public const string Checkout = "checkout";
    public const string Clear = "clear";
    public const string Remove = "remove";
    public const string MarkRead = "mark-read";
    public const string Reply = "reply";
    public const string Complete = "complete";
    public const string Suspend = "suspend";
    public const string Services = "services";
    public const string Service = "service";
    public const string Roles = "roles";
    public const string AssignRoles = "assign-roles";
    public const string Permissions = "permissions";
    public const string AssignPermissions = "assign-permissions";
    public const string PermissionOverrides = "permission-overrides";
    public const string ChangePassword = "change-password";
    public const string UpdateImage = "update-image";

    public static readonly IReadOnlyDictionary<string, string> Descriptions = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        [Delete] = "Deletes the resource.",
        [Cancel] = "Cancels the resource (an unpaid order, a visit, a membership at period end, a pending invite or a tenant).",
        [Pay] = "Starts or resumes paying the resource; the response carries what the payment client needs.",
        [Refund] = "Refunds a paid order in full (staff).",
        [Void] = "Voids an open invoice (staff).",
        [MarkPaid] = "Records an offline payment for an open invoice (staff).",
        [Pdf] = "Downloads the resource as a PDF (base64 in the envelope).",
        [Confirm] = "Confirms a requested visit (staff).",
        [AssignTechnician] = "Assigns a technician to an open visit (staff).",
        [WorkOrder] = "The work order recorded for a visit.",
        [WorkOrders] = "The work orders recorded for a unit of equipment.",
        [Appointment] = "The visit this resource belongs to.",
        [AddMedia] = "Attaches a photo to the resource.",
        [Media] = "The media asset (photo) the item points at.",
        [Pause] = "Pauses billing of an active membership.",
        [Resume] = "Resumes a paused membership.",
        [Reactivate] = "Undoes a pending cancellation, or reactivates a suspended tenant.",
        [ChangePlan] = "Switches an active membership to another plan.",
        [Plan] = "The membership plan of a membership.",
        [Subscribe] = "Subscribes the caller to the plan.",
        [SetActive] = "Shows or hides the resource for customers (staff).",
        [SetShipping] = "Makes the address the default shipping address.",
        [SetBilling] = "Makes the address the default billing address.",
        [ChangeAddresses] = "Points an unpaid order at the caller's current default addresses.",
        [Checkout] = "Places an order from the basket.",
        [Clear] = "Removes every item from the basket.",
        [Remove] = "Removes the item from its parent resource.",
        [MarkRead] = "Marks the notification as read.",
        [Reply] = "Replies to the message by email (staff).",
        [Complete] = "Completes a pending referral and pays its reward (staff).",
        [Suspend] = "Suspends an active tenant (platform staff).",
        [Services] = "The services in a category.",
        [Service] = "The service the resource belongs to.",
        [Roles] = "The roles of a user.",
        [AssignRoles] = "Replaces the roles of a user.",
        [Permissions] = "The permissions granted to a role.",
        [AssignPermissions] = "Replaces the permissions granted to a role.",
        [PermissionOverrides] = "Per-user permission overrides on top of the user's roles.",
        [ChangePassword] = "Changes the caller's password.",
        [UpdateImage] = "Replaces the caller's profile image.",
    };
}
