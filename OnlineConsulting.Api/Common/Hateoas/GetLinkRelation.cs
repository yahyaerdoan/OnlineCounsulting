namespace OnlineConsulting.Api.Common.Hateoas;

/// <summary>Documentation target of the "oc" CURIE: what an "oc:{rel}" link relation means.</summary>
public class GetLinkRelation : IVersionNeutralEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/rels/{rel}", Handle)
            .WithTags("Hypermedia")
            .WithName("GetLinkRelation")
            .WithDescription("Describes an application link relation used as \"oc:{rel}\" in \"_links\".");
    }

    private static IResult Handle(string rel)
        => Rels.Descriptions.TryGetValue(rel, out var description)
            ? Results.Ok(new { rel = $"{Rels.CurieName}:{rel}", description })
            : Results.NotFound();
}
