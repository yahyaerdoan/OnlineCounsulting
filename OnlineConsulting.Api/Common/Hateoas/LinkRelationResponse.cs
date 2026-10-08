namespace OnlineConsulting.Api.Common.Hateoas;

/// <summary>What an "oc:{name}" link relation means, served at /rels/{name}.</summary>
public sealed record LinkRelationResponse(string Rel, string Description);
