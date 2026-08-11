using Microsoft.Extensions.Options;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using System.Text.Json;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class RecaptchaManager(HttpClient httpClient, IOptions<AppSettingRecaptchaOption> recaptchaOption) : IRecaptchaService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AppSettingRecaptchaOption _option = recaptchaOption.Value;

    public async Task<bool> VerifyAsync(string? recaptchaResponse)
    {
        if (string.IsNullOrWhiteSpace(recaptchaResponse) || string.IsNullOrWhiteSpace(_option.SecretKey))
            return false;

        var parameters = new Dictionary<string, string>
        {
            ["secret"] = _option.SecretKey,
            ["response"] = recaptchaResponse
        };

        using var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.TryGetProperty("success", out var successProperty) && successProperty.GetBoolean();
    }
}
