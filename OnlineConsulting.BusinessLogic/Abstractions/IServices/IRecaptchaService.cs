namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IRecaptchaService
{
    Task<bool> VerifyAsync(string? recaptchaResponse);
}
