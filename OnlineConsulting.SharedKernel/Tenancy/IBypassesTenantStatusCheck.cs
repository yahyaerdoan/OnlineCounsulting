namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Opts a request out of TenantStatusCheckBehavior - for the one command a blocked tenant must still be able to call: retrying its own billing.</summary>
public interface IBypassesTenantStatusCheck;
