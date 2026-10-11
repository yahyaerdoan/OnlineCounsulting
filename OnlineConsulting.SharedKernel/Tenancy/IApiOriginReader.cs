namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>The Api's own public origin (not a tenant's or the platform's website - a different port in dev). For backend-to-backend callback URLs, e.g. the Stripe Connect OAuth redirect_uri, which must point at the Api, not the web frontend.</summary>
public interface IApiOriginReader
{
    string GetOrigin();
}
