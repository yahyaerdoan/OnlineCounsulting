using Microsoft.OpenApi;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OnlineConsulting.Api.Common;

public static class SwaggerExtensions
{
    public static RouteHandlerBuilder WithPlaceholderExample<TDto>(this RouteHandlerBuilder builder, TDto example) where TDto : class
    {
        return builder.AddOpenApiOperationTransformer((op, context, ct) =>
        {
            var requestBody = op.RequestBody ??= new OpenApiRequestBody();
            requestBody.Content!["application/json"] = new OpenApiMediaType
            {
                Example = JsonSerializer.SerializeToNode(example)
            };
            return Task.CompletedTask;
        });
    }

    public static RouteHandlerBuilder WithParamExamples(this RouteHandlerBuilder b, object pairs)
    {
        var dict = pairs.GetType().GetProperties()
                        .ToDictionary(p => p.Name, p => p.GetValue(pairs));
        return b.AddOpenApiOperationTransformer((op, context, ct) =>
        {
            foreach (var (k, v) in dict)
            {
                if (op.Parameters?.FirstOrDefault(x => x.Name == k) is OpenApiParameter p)
                    p.Example = ToJsonNode(v);
            }
            return Task.CompletedTask;
        });
    }

    private static JsonNode? ToJsonNode(object? value) =>
        value switch
        {
            null => null,
            string s => JsonValue.Create(s),
            bool b => JsonValue.Create(b),
            int i => JsonValue.Create(i),
            long l => JsonValue.Create(l),
            float f => JsonValue.Create(f),
            double d => JsonValue.Create(d),
            decimal m => JsonValue.Create(m),
            DateTime dt => JsonValue.Create(dt.ToString("o")),
            DateTimeOffset dto => JsonValue.Create(dto.ToString("o")),
            Guid g => JsonValue.Create(g.ToString()),
            _ => JsonValue.Create(value.ToString() ?? string.Empty)
        };
}
