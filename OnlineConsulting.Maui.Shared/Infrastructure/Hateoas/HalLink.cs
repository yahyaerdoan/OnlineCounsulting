namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>One HAL link: where an action lives and which HTTP method it takes.</summary>
public sealed record HalLink(string Href, string? Method = null)
{
    /// <summary>The method to call the link with; GET when the Api didn't say.</summary>
    public HttpMethod HttpMethod => Method is { Length: > 0 } method ? new HttpMethod(method) : HttpMethod.Get;

    /// <summary>Path and query only, sent to the client's own Api origin, so a token never follows a link to another host.</summary>
    public string RelativePath => Uri.TryCreate(Href, UriKind.Absolute, out var absolute) ? absolute.PathAndQuery : Href;
}
