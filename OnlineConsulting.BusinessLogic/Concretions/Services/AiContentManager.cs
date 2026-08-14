using Microsoft.Extensions.Configuration;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class AiContentManager(IConfiguration configuration, HttpClient httpClient) : IAiContentService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task<IOperationResult<(string slug, string description)>> GenerateServiceContentAsync(string title)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return new ErrorDataResult<(string slug, string description)>("OpenAI:ApiKey is not configured.", ResultStatus.InternalServerError);

        //Build robust prompt
        var payload = new
        {
            model = "gpt-4o",
            temperature = 0.2,
            messages = new[]
            {
                new {
                    role = "system",
                    content = "You are a strict JSON API. Respond ONLY with a single JSON object. No explanations, no markdown."
                },
                new {
                    role = "user",
                    content = $@"Generate an SEO-friendly slug and a detailed description for this title:

                                Title: ""{title}""

                                Requirements:
                                - Slug: lowercase, kebab-case, only hyphens allowed.
                                - Description: 6–8 clear sentences, no bullet points.

                                Respond ONLY in this JSON format:
                                {{
                                  ""slug"": ""your-generated-slug"",
                                  ""description"": ""your-generated-description""
                                }}"
                }
            }
        };

        //Build request
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        //Send & ensure success
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return new ErrorDataResult<(string slug, string description)>("The OpenAI request failed.", ResultStatus.BadGateway);

        var jsonResponse = await response.Content.ReadAsStringAsync();

        //Extract raw model reply
        using var doc = JsonDocument.Parse(jsonResponse);
        var rawContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(rawContent))
            return new ErrorDataResult<(string slug, string description)>("OpenAI returned empty content.", ResultStatus.BadGateway);

        //Clean up code fences, escaped quotes, stray backticks
        var cleanJson = rawContent.Trim();

        if (cleanJson.StartsWith("```"))
        {
            var firstNewline = cleanJson.IndexOf('\n');
            if (firstNewline > 0)
                cleanJson = cleanJson[(firstNewline + 1)..];

            if (cleanJson.EndsWith("```"))
                cleanJson = cleanJson[..^3];
        }

        //Remove accidental extra wrapping quotes
        cleanJson = cleanJson.Trim();
        if (cleanJson.StartsWith('"') && cleanJson.EndsWith('"'))
            cleanJson = cleanJson.Trim('"');

        //Replace escaped quotes if any
        cleanJson = cleanJson.Replace("\\\"", "\"");

        //Safe parse using JsonDocument (no DTO risk)
        string slug;
        string description;

        try
        {
            using var parsed = JsonDocument.Parse(cleanJson);
            var root = parsed.RootElement;

            slug = root.GetProperty("slug").GetString()?.Trim() ?? string.Empty;
            description = root.GetProperty("description").GetString()?.Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<(string slug, string description)>(
                $"Failed to parse OpenAI response as JSON.\nRaw: {cleanJson}\nError: {ex.Message}",
                ResultStatus.BadGateway);
        }

        return new SuccessDataResult<(string slug, string description)>((slug, description), "AI content generated successfully.");
    }
}

public class SlugDescriptionDto
{
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
