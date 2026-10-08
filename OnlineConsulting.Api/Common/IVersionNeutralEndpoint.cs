namespace OnlineConsulting.Api.Common;

/// <summary>An endpoint whose address is fixed outside the API's versioning, such as a payment provider webhook or a link relation document.</summary>
public interface IVersionNeutralEndpoint : IEndpoint
{
}
