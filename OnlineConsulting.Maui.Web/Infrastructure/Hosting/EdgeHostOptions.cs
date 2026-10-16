namespace OnlineConsulting.Maui.Web.Infrastructure.Hosting;

/// <summary>Bound from the Edge config section; empty Secret turns the edge headers off.</summary>
public class EdgeHostOptions
{
    /// <summary>Shared with the edge Worker (its EDGE_SECRET); proves a request came through it.</summary>
    public string Secret { get; set; } = string.Empty;
}
