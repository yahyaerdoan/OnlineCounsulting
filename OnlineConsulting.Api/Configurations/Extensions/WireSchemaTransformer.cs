using Hateoas;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.Api.Configurations.Extensions;

/// <summary>
/// Describes JSON that custom converters or lenient number reading hide from the schema generator as it is actually written, so the
/// OpenAPI document and clients generated from it get real types: <see cref="ResultStatus"/> is a number, HAL "_links" is a map of
/// links plus "curies", and numbers are never strings.
/// </summary>
public sealed class WireSchemaTransformer : IOpenApiSchemaTransformer
{
    private const JsonSchemaType Numeric = JsonSchemaType.Integer | JsonSchemaType.Number;

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        if (type == typeof(ResultStatus))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
        }
        else if (type == typeof(LinkCollection))
        {
            DescribeLinks(schema);
        }
        else if (schema.Type is { } schemaType && (schemaType & Numeric) != 0 && (schemaType & JsonSchemaType.String) != 0)
        {
            schema.Type = schemaType & ~JsonSchemaType.String;
            schema.Pattern = null;
        }

        return Task.CompletedTask;
    }

    private static void DescribeLinks(OpenApiSchema schema)
    {
        schema.Type = JsonSchemaType.Object;
        schema.Description = "HAL links by relation (IANA names such as \"self\", application ones as \"oc:{name}\").";
        schema.Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["curies"] = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Required = new HashSet<string> { "name", "href" },
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                        ["href"] = new OpenApiSchema { Type = JsonSchemaType.String },
                        ["templated"] = new OpenApiSchema { Type = JsonSchemaType.Boolean },
                    },
                },
            },
        };
        schema.AdditionalProperties = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "href" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["href"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["method"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["title"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["templated"] = new OpenApiSchema { Type = JsonSchemaType.Boolean },
            },
        };
    }
}
