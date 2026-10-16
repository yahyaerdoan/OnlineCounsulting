namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Railway service that serves tenant sites; the token is a project token for that environment.</summary>
public class RailwayDomainOptions
{
    public string ApiUrl { get; set; } = "https://backboard.railway.com/graphql/v2";
    public string ProjectToken { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string EnvironmentId { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public int? TargetPort { get; set; }
}
