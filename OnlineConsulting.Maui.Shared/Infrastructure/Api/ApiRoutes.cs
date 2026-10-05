namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Api route paths, shared by every caller so a rename touches one place.</summary>
public static class ApiRoutes
{
    /// <summary>Root of the current API version; every route below starts here.</summary>
    public const string V1 = "/api/v1";

    /// <summary>?index=&amp;size= for any paginated /query endpoint.</summary>
    public static string Paged(string basePath, int index, int size) => $"{basePath}?index={index}&size={size}";

    public static class Auth
    {
        public const string Base = V1 + "/auth";

        public const string Login = Base + "/login";
        public const string Refresh = Base + "/refresh";
        public const string AcceptInvite = Base + "/invites/accept";
        public const string Register = Base + "/register";
        public const string ConfirmEmail = Base + "/confirm-email";
        public const string ForgotPassword = Base + "/forgot-password";
        public const string ResetPassword = Base + "/reset-password";
    }

    /// <summary>Invite-a-teammate route - the invitee sets their own password, not the admin.</summary>
    public static class Invites
    {
        public const string Create = V1 + "/auth/invites";
        public const string All = V1 + "/invites/query";

        public static string ById(Guid id) => $"{V1}/invites/{id}";
    }

    public static class Permissions
    {
        public const string All = V1 + "/permissions";
    }

    public static class Notifications
    {
        public const string Base = V1 + "/notifications";

        /// <summary>GET the caller's inbox, newest first, paginated.</summary>
        public const string UnreadCount = Base + "/unread-count";
        public const string ReadAll = Base + "/read-all";

        public static string Read(Guid id) => $"{Base}/{id}/read";
    }

    public static class DeviceTokens
    {
        public const string Base = V1 + "/device-tokens";

        /// <summary>POST registers the signed-in user's push token for this device; DELETE ById(token) removes it on sign-out.</summary>

        public static string ById(string token) => $"{Base}/{Uri.EscapeDataString(token)}";
    }

    public static class Users
    {
        public const string Base = V1 + "/users";

        public const string Me = Base + "/me";
        public const string All = Base + "/query";

        public static string ById(Guid id) => $"{Base}/{id}";
        public static string Roles(Guid id) => $"{Base}/{id}/roles";
        public static string PermissionOverrides(Guid id) => $"{Base}/{id}/permission-overrides";
    }

    public static class Roles
    {
        public const string Base = V1 + "/roles";

        public const string All = Base + "/query";

        /// <summary>GET for the flat dropdown list, POST for create.</summary>

        public static string ById(Guid id) => $"{Base}/{id}";
        public static string Permissions(Guid id) => $"{Base}/{id}/permissions";

        /// <summary>Every role's permissions in one call - backs the permission matrix page.</summary>
        public const string PermissionsMatrix = Base + "/permissions";
    }

    public static class Categories
    {
        public const string Base = V1 + "/categories";

        public const string All = Base + "/query";

        public static string ById(Guid id) => $"{Base}/{id}";
    }

    public static class Services
    {
        public const string Base = V1 + "/services";

        public const string All = Base + "/query";
        public const string Featured = Base + "/featured";
        public const string Search = Base + "/search";
        public const string MediaItems = Base + "/media-items";

        public static string ById(Guid id) => $"{Base}/{id}";
        public static string BySlug(string slug) => $"{Base}/by-slug/{slug}";
        public static string RemoveMediaItem(Guid id) => $"{Base}/media-items/{id}";

        /// <summary>Paginated - a category's services, not the flat list.</summary>
        public static string ByCategory(Guid categoryId) => $"{V1}/categories/{categoryId}/services";
    }

    public static class Media
    {
        public const string Base = V1 + "/media";

        public const string Upload = Base;

        public static string ById(Guid id) => $"{Base}/{id}";
    }

    public static class SiteContent
    {
        public static class AboutUs
        {
            public const string Base = V1 + "/site-content/about-us";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class FooterInfo
        {
            public const string Base = V1 + "/site-content/footer-info";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class GalleryCategory
        {
            public const string Base = V1 + "/site-content/gallery-categories";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class GalleryItem
        {
            public const string Base = V1 + "/site-content/gallery-items";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class ServiceProcessStep
        {
            public const string Base = V1 + "/site-content/service-process-steps";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class ServiceOffering
        {
            public const string Base = V1 + "/site-content/service-offerings";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class ServiceArea
        {
            public const string Base = V1 + "/site-content/service-areas";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class HeroSlide
        {
            public const string Base = V1 + "/site-content/hero-slides";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class SocialLink
        {
            public const string Base = V1 + "/site-content/social-links";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Testimonial
        {
            public const string Base = V1 + "/site-content/testimonials";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class FeatureHighlight
        {
            public const string Base = V1 + "/site-content/feature-highlights";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Partnership
        {
            public const string Base = V1 + "/site-content/partnerships";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class FeatureHighlightsIntro
        {
            public const string Base = V1 + "/site-content/feature-highlights-intro";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class PartnershipSocialLink
        {
            public const string Base = V1 + "/site-content/partnership-social-links";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class FaqItems
        {
            public const string Base = V1 + "/site-content/faq-items";

            public const string All = Base + "/query";
            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class PageBanners
        {
            public const string Base = V1 + "/site-content/page-banners";

            public const string All = Base;
            public static string ById(Guid id) => $"{Base}/{id}";
        }
    }

    public static class Inquiries
    {
        public static class Contact
        {
            public const string Base = V1 + "/contact";

            public const string Get = Base;
            public const string Update = Base;
        }

        public static class Messages
        {
            public const string Base = V1 + "/inquiries/messages";

            public const string All = Base + "/query";

            /// <summary>POST submits a contact-form message - public, no login required.</summary>
            public const string Submit = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
            public static string Reply(Guid id) => $"{Base}/{id}/reply";
        }

        public static class Newsletter
        {
            public const string Base = V1 + "/inquiries/newsletter";

            public const string All = Base + "/query";

            /// <summary>POST subscribes an email - public, no login required.</summary>
            public const string Subscribe = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
        }
    }

    public static class Scheduling
    {
        public static class AvailabilityRule
        {
            public const string Base = V1 + "/scheduling/availability-rules";

            public const string All = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Availability
        {
            public const string Base = V1 + "/scheduling/availability";

            /// <summary>GET free time slots for a date - public, no login required to browse.</summary>
        }
    }

    public static class Commerce
    {
        public static class Orders
        {
            public const string Base = V1 + "/orders";

            public const string All = Base + "/admin/query";

            /// <summary>GET the caller's own orders (current-user-scoped, not paginated).</summary>
            public const string Mine = Base;

            /// <summary>POST creates an Order from the caller's current basket - requires auth, no body,
            /// requires shipping and billing addresses already set (see Addresses.SetShipping/SetBilling).</summary>
            public const string Checkout = Base + "/checkout";

            public static string ById(Guid id) => $"{Base}/{id}";

            /// <summary>Re-fetches a fresh PaymentClientSecret for an already-created, still-unpaid order -
            /// lets Checkout.razor resume the Stripe payment step after a page reload.</summary>
            public static string ResumePayment(Guid id) => $"{Base}/{id}/resume-payment";

            /// <summary>PUT: re-points an unpaid order at the caller's current default shipping/billing addresses.</summary>
            public static string UpdateAddresses(Guid id) => $"{Base}/{id}/addresses";
        }

        public static class Baskets
        {
            public const string Base = V1 + "/basket";

            /// <summary>GET the current basket, DELETE clears it - guest/user resolved server-side.</summary>

            /// <summary>POST adds a service, increasing its quantity if already in the basket (JSON body, guest/user resolved server-side).</summary>
            public const string Items = Base + "/items";

            /// <summary>GET the row count of basket items - safe to call unconditionally, 0 if no basket yet.</summary>
            public const string Count = Base + "/count";

            /// <summary>DELETE a single line by basket item id (not ServiceId).</summary>
            public static string RemoveItem(Guid id) => $"{Base}/items/{id}";

            /// <summary>PUT sets a line's quantity to an absolute value - the cart page's +/- stepper.</summary>
            public static string SetItemQuantity(Guid id) => $"{Base}/items/{id}";
        }
    }

    public static class Invoices
    {
        public const string Base = V1 + "/invoices";

        public const string Mine = Base + "/mine";
        public const string All = Base + "/admin/query";

        public static string ById(Guid id) => $"{Base}/{id}";

        /// <summary>POST: settles the caller's open invoice at once if the provider already took the payment.</summary>
        public static string SyncPayment(Guid id) => $"{Base}/{id}/sync-payment";
    }

    public static class Addresses
    {
        public const string Base = V1 + "/addresses";

        /// <summary>GET (list), POST (create) - both current-user-scoped.</summary>

        /// <summary>GET ?text= - US street address type-ahead (signed-in only).</summary>
        public static string Suggestions(string text) => $"{Base}/suggestions?text={Uri.EscapeDataString(text)}";

        /// <summary>GET the current user's billing address - 404 if not set yet.</summary>
        public const string Billing = Base + "/billing";

        /// <summary>GET the current user's shipping address - 404 if not set yet.</summary>
        public const string Shipping = Base + "/shipping";

        /// <summary>PUT (update), DELETE.</summary>
        public static string ById(Guid id) => $"{Base}/{id}";

        /// <summary>PUT - marks the given address as the current user's billing address.</summary>
        public static string SetBilling(Guid id) => $"{Base}/{id}/billing";

        /// <summary>PUT - marks the given address as the current user's shipping address.</summary>
        public static string SetShipping(Guid id) => $"{Base}/{id}/shipping";
    }

    public static class Settings
    {
        public static class FeatureFlags
        {
            public const string Base = V1 + "/admin/feature-flags";

            public const string Get = Base;

            public static string Set(string key) => $"{Base}/{key}";
        }
    }

    public static class Operations
    {
        public static class Equipment
        {
            public const string Base = V1 + "/equipment";

            public const string All = Base + "/query";
            /// <summary>GET the caller's own equipment - the customer portal's equipment panel.</summary>
            public const string Mine = Base + "/mine";

            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Appointments
        {
            public const string Base = V1 + "/appointments";

            public const string All = Base + "/admin/query";
            /// <summary>POST creates an appointment - pass ServiceId to book a service, omit it for a general meeting request.</summary>
            /// <summary>GET the caller's own appointments, paginated.</summary>
            public const string Mine = Base + "/mine";

            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class WorkOrders
        {
            public const string Base = V1 + "/work-orders";

            public static string ByAppointmentId(Guid appointmentId) => $"{V1}/appointments/{appointmentId}/work-order";
            public static string ByEquipmentId(Guid equipmentId) => $"{V1}/equipment/{equipmentId}/work-orders";
            public static string AddMediaItem(Guid workOrderId) => $"{Base}/{workOrderId}/media-items";
        }
    }

    public static class Growth
    {
        public static class MembershipPlans
        {
            public const string Base = V1 + "/membership-plans";

            public const string All = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
            public static string SetActive(Guid id, bool isActive) => $"{Base}/{id}/active?isActive={isActive}";
        }

        public static class CustomerMemberships
        {
            public const string Base = V1 + "/memberships";

            public const string All = Base + "/query";

            /// <summary>GET the caller's own membership - 404 means not currently a member.</summary>
            public const string Mine = Base + "/mine";

            public const string Cancel = Base + "/cancel";

            public const string Subscribe = Base + "/subscribe";

            public static string ChangePlan(Guid newMembershipPlanId) => $"{Base}/change-plan?newMembershipPlanId={newMembershipPlanId}";

            public const string Pause = Base + "/pause";

            public const string Resume = Base + "/resume";

            /// <summary>Admin-cancel a specific customer's membership by CustomerMembership.Id.</summary>
            public static string AdminCancel(Guid id) => $"{Base}/{id}/cancel";

            /// <summary>Undoes a pending cancellation before the period ends.</summary>
            public const string Reactivate = Base + "/reactivate";

            public static string AdminReactivate(Guid id) => $"{Base}/{id}/reactivate";

            /// <summary>GET the caller's most recent ended membership - 404 when none or already a member.</summary>
            public const string MinePrevious = Base + "/mine/previous";

            /// <summary>POST previews a promo code's discount for the current user - does not redeem it.</summary>
            public const string ValidatePromoCode = Base + "/promo-codes/validate";
        }

        public static class PromoCodes
        {
            public const string Base = V1 + "/promo-codes";

            public const string All = Base;

            public static string SetActive(Guid id, bool isActive) => $"{Base}/{id}/active?isActive={isActive}";
        }

        public static class Referrals
        {
            public const string Base = V1 + "/referrals";

            public const string All = Base + "/query";

            public static string Complete(Guid id) => $"{Base}/{id}/complete";

            /// <summary>POST - gets or creates the caller's own referral code, idempotent after first call.</summary>
            public const string MyCode = Base + "/my-code";

            /// <summary>GET the caller's own list of people they referred.</summary>
            public const string Mine = Base + "/mine";

            /// <summary>GET the caller's own account-credit balance and ledger.</summary>
            public const string MyCredit = Base + "/my-credit";
        }

        public static class Promotions
        {
            public const string Base = V1 + "/site-content/promotions";

            public const string All = Base + "/query";

            public static string ById(Guid id) => $"{Base}/{id}";
        }
    }

    /// <summary>Public, unauthenticated - tenant self-service signup.</summary>
    public static class Tenancy
    {
        public const string Base = V1 + "/tenancy";

        public const string ModuleOfferings = Base + "/module-offerings";
        public const string Bundles = Base + "/bundles";
        public const string Signup = Base + "/signup";

        /// <summary>Authenticated retry when signup succeeded (admin account created) but billing failed.</summary>
        public static string Activate(Guid tenantId) => $"{Base}/{tenantId}/activate";

        /// <summary>Authenticated - the caller's own tenant (any tenant admin, not just SuperAdmin).</summary>
        public const string MyTenant = Base + "/my-tenant";

        /// <summary>Tenant admins - sets the caller's own business time zone.</summary>
        public const string MyTimeZone = Base + "/my-tenant/time-zone";

        /// <summary>Public - the IANA time zone of the caller's business (the default tenant's when anonymous).</summary>
        public const string TimeZone = Base + "/time-zone";
    }

    public static class Platform
    {
        public static class ModuleOfferings
        {
            public const string Base = V1 + "/tenancy/admin/module-offerings";

            public const string All = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Bundles
        {
            public const string Base = V1 + "/tenancy/admin/bundles";

            public const string All = Base;

            public static string ById(Guid id) => $"{Base}/{id}";
        }

        public static class Tenants
        {
            public const string Base = V1 + "/tenancy";

            public const string All = Base + "/admin/tenants/query";

            public static string ById(Guid tenantId) => $"{Base}/admin/tenants/{tenantId}";
            public static string Suspend(Guid tenantId) => $"{Base}/admin/tenants/{tenantId}/suspend";
            public static string Reactivate(Guid tenantId) => $"{Base}/admin/tenants/{tenantId}/reactivate";
            public static string Cancel(Guid tenantId) => $"{Base}/admin/tenants/{tenantId}/cancel";
            public static string AddModule(Guid tenantId, string key) => $"{Base}/{tenantId}/modules/{key}";
            public static string RemoveModule(Guid tenantId, string key) => $"{Base}/{tenantId}/modules/{key}";
        }
    }
}
