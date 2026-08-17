namespace OnlineConsulting.UserInterface.Features.Account;

public interface IRecaptchaService
{
    Task<bool> VerifyAsync(string? recaptchaResponse);
}
