using Microsoft.Extensions.Options;
using System.Text.Json;

namespace OnlineConsulting.UserInterface.Features.Account;

public class RecaptchaService(HttpClient httpClient, IOptions<RecaptchaOptions> recaptchaOptions) : IRecaptchaService
{
    private readonly RecaptchaOptions _option = recaptchaOptions.Value;

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
        var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.TryGetProperty("success", out var successProperty) && successProperty.GetBoolean();
    }
}
