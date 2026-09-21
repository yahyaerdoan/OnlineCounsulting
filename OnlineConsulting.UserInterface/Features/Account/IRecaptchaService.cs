namespace OnlineConsulting.UserInterface.Features.Account;

public interface IRecaptchaService
{
    /// <summary>Verifies a reCAPTCHA response token against Google's siteverify endpoint.</summary>
    Task<bool> VerifyAsync(string? recaptchaResponse);
}
