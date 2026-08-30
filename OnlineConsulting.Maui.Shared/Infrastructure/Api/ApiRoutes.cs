namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Api route paths, shared by every caller so a rename touches one place.</summary>
public static class ApiRoutes
{
    /// <summary>?index=&amp;size= for any paginated /query endpoint.</summary>
    public static string Paged(string basePath, int index, int size) => $"{basePath}?index={index}&size={size}";

    public static class Auth
    {
        public const string Login = "/api/auth/login";
        public const string Refresh = "/api/auth/refresh";
        public const string AcceptInvite = "/api/auth/invites/accept";
    }

    /// <summary>Invite-a-teammate route - the invitee sets their own password, not the admin.</summary>
    public static class Invites
    {
        public const string Create = "/api/auth/invites";
        public const string All = "/api/invites/query";

        public static string ById(Guid id) => $"/api/invites/{id}";
    }

    public static class Permissions
    {
        public const string All = "/api/permissions";
    }

    public static class Users
    {
        public const string Me = "/api/users/me";
        public const string All = "/api/users/query";

        public static string ById(Guid id) => $"/api/users/{id}";
        public static string Roles(Guid id) => $"/api/users/{id}/roles";
    }

    public static class Roles
    {
        public const string All = "/api/roles/query";

        /// <summary>GET for the flat dropdown list, POST for create.</summary>
        public const string Base = "/api/roles";

        public static string ById(Guid id) => $"/api/roles/{id}";
        public static string Permissions(Guid id) => $"/api/roles/{id}/permissions";

        /// <summary>Every role's permissions in one call - backs the permission matrix page.</summary>
        public const string PermissionsMatrix = "/api/roles/permissions";
    }

    public static class Categories
    {
        public const string All = "/api/categories/query";
        public const string Base = "/api/categories";

        public static string ById(Guid id) => $"/api/categories/{id}";
    }

    public static class Services
    {
        public const string All = "/api/services/query";
        public const string Base = "/api/services";
        public const string MediaItems = "/api/services/media-items";

        public static string ById(Guid id) => $"/api/services/{id}";
        public static string RemoveMediaItem(Guid id) => $"/api/services/media-items/{id}";
    }

    public static class Media
    {
        public const string Upload = "/api/media";
        public const string List = "/api/media";

        public static string ById(Guid id) => $"/api/media/{id}";
    }

    public static class SiteContent
    {
        public static class AboutUs
        {
            public const string All = "/api/site-content/about-us/query";
            public const string Base = "/api/site-content/about-us";
            public static string ById(Guid id) => $"/api/site-content/about-us/{id}";
        }

        public static class FooterInfo
        {
            public const string All = "/api/site-content/footer-info/query";
            public const string Base = "/api/site-content/footer-info";
            public static string ById(Guid id) => $"/api/site-content/footer-info/{id}";
        }

        public static class GalleryCategory
        {
            public const string All = "/api/site-content/gallery-categories/query";
            public const string Base = "/api/site-content/gallery-categories";
            public static string ById(Guid id) => $"/api/site-content/gallery-categories/{id}";
        }

        public static class GalleryItem
        {
            public const string All = "/api/site-content/gallery-items/query";
            public const string Base = "/api/site-content/gallery-items";
            public static string ById(Guid id) => $"/api/site-content/gallery-items/{id}";
        }

        public static class ServiceProcessStep
        {
            public const string All = "/api/site-content/service-process-steps/query";
            public const string Base = "/api/site-content/service-process-steps";
            public static string ById(Guid id) => $"/api/site-content/service-process-steps/{id}";
        }

        public static class ServiceOffering
        {
            public const string All = "/api/site-content/service-offerings/query";
            public const string Base = "/api/site-content/service-offerings";
            public static string ById(Guid id) => $"/api/site-content/service-offerings/{id}";
        }

        public static class ServiceArea
        {
            public const string All = "/api/site-content/service-areas/query";
            public const string Base = "/api/site-content/service-areas";
            public static string ById(Guid id) => $"/api/site-content/service-areas/{id}";
        }

        public static class HeroSlide
        {
            public const string All = "/api/site-content/hero-slides/query";
            public const string Base = "/api/site-content/hero-slides";
            public static string ById(Guid id) => $"/api/site-content/hero-slides/{id}";
        }

        public static class SocialLink
        {
            public const string All = "/api/site-content/social-links/query";
            public const string Base = "/api/site-content/social-links";
            public static string ById(Guid id) => $"/api/site-content/social-links/{id}";
        }

        public static class Testimonial
        {
            public const string All = "/api/site-content/testimonials/query";
            public const string Base = "/api/site-content/testimonials";
            public static string ById(Guid id) => $"/api/site-content/testimonials/{id}";
        }

        public static class FeatureHighlight
        {
            public const string All = "/api/site-content/feature-highlights/query";
            public const string Base = "/api/site-content/feature-highlights";
            public static string ById(Guid id) => $"/api/site-content/feature-highlights/{id}";
        }

        public static class Partnership
        {
            public const string All = "/api/site-content/partnerships/query";
            public const string Base = "/api/site-content/partnerships";
            public static string ById(Guid id) => $"/api/site-content/partnerships/{id}";
        }

        public static class PartnershipSocialLink
        {
            public const string Base = "/api/site-content/partnership-social-links";
            public static string ById(Guid id) => $"/api/site-content/partnership-social-links/{id}";
        }

        public static class FaqItems
        {
            public const string All = "/api/site-content/faq-items/query";
            public const string Base = "/api/site-content/faq-items";
            public static string ById(Guid id) => $"/api/site-content/faq-items/{id}";
        }

        public static class PageBanners
        {
            public const string All = "/api/site-content/page-banners";
            public const string Base = "/api/site-content/page-banners";
            public static string ById(Guid id) => $"/api/site-content/page-banners/{id}";
        }
    }

    public static class Inquiries
    {
        public static class Contact
        {
            public const string Get = "/api/contact";
            public const string Update = "/api/contact";
        }

        public static class Messages
        {
            public const string All = "/api/inquiries/messages/query";

            public static string ById(Guid id) => $"/api/inquiries/messages/{id}";
            public static string Reply(Guid id) => $"/api/inquiries/messages/{id}/reply";
        }

        public static class Newsletter
        {
            public const string All = "/api/inquiries/newsletter/query";

            public static string ById(Guid id) => $"/api/inquiries/newsletter/{id}";
        }
    }

    public static class Scheduling
    {
        public static class AvailabilityRule
        {
            public const string All = "/api/scheduling/availability-rules";
            public const string Base = "/api/scheduling/availability-rules";

            public static string ById(Guid id) => $"/api/scheduling/availability-rules/{id}";
        }
    }

    public static class Commerce
    {
        public static class Orders
        {
            public const string All = "/api/orders/admin/query";

            public static string Refund(Guid id) => $"/api/orders/{id}/refund";
        }
    }

    public static class Settings
    {
        public static class FeatureFlags
        {
            public const string Get = "/api/admin/feature-flags";

            public static string Set(string key) => $"/api/admin/feature-flags/{key}";
        }
    }

    public static class Operations
    {
        public static class Equipment
        {
            public const string All = "/api/equipment/query";
            public const string Base = "/api/equipment";

            public static string ById(Guid id) => $"/api/equipment/{id}";
        }

        public static class Appointments
        {
            public const string All = "/api/appointments/admin/query";

            public static string ById(Guid id) => $"/api/appointments/{id}";
            public static string Confirm(Guid id) => $"/api/appointments/{id}/confirm";
            public static string Cancel(Guid id) => $"/api/appointments/{id}/cancel";
            public static string AssignTechnician(Guid id) => $"/api/appointments/{id}/assign-technician";
        }

        public static class WorkOrders
        {
            public const string Base = "/api/work-orders";

            public static string ByAppointmentId(Guid appointmentId) => $"/api/appointments/{appointmentId}/work-order";
            public static string ByEquipmentId(Guid equipmentId) => $"/api/equipment/{equipmentId}/work-orders";
            public static string AddMediaItem(Guid workOrderId) => $"/api/work-orders/{workOrderId}/media-items";
        }
    }

    public static class Growth
    {
        public static class MembershipPlans
        {
            public const string All = "/api/membership-plans";
            public const string Base = "/api/membership-plans";

            public static string ById(Guid id) => $"/api/membership-plans/{id}";
        }

        public static class CustomerMemberships
        {
            public const string All = "/api/memberships/query";
        }

        public static class Referrals
        {
            public const string All = "/api/referrals/query";

            public static string Complete(Guid id) => $"/api/referrals/{id}/complete";
        }

        public static class Promotions
        {
            public const string All = "/api/site-content/promotions/query";
            public const string Base = "/api/site-content/promotions";

            public static string ById(Guid id) => $"/api/site-content/promotions/{id}";
        }
    }

    public static class Platform
    {
        public static class ModuleOfferings
        {
            public const string All = "/api/tenancy/admin/module-offerings";
            public const string Base = "/api/tenancy/admin/module-offerings";

            public static string ById(Guid id) => $"/api/tenancy/admin/module-offerings/{id}";
        }

        public static class Bundles
        {
            public const string All = "/api/tenancy/admin/bundles";
            public const string Base = "/api/tenancy/admin/bundles";

            public static string ById(Guid id) => $"/api/tenancy/admin/bundles/{id}";
        }

        public static class Tenants
        {
            public const string All = "/api/tenancy/admin/tenants/query";

            public static string ById(Guid tenantId) => $"/api/tenancy/admin/tenants/{tenantId}";
            public static string Suspend(Guid tenantId) => $"/api/tenancy/admin/tenants/{tenantId}/suspend";
            public static string Reactivate(Guid tenantId) => $"/api/tenancy/admin/tenants/{tenantId}/reactivate";
            public static string AddModule(Guid tenantId, string key) => $"/api/tenancy/{tenantId}/modules/{key}";
            public static string RemoveModule(Guid tenantId, string key) => $"/api/tenancy/{tenantId}/modules/{key}";
        }
    }
}
