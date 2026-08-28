using FluentValidation;
using FluentValidation.Resources;

namespace OnlineConsulting.SharedKernel.Validation;

/// <summary>Overrides FluentValidation's default quoted messages with plain phrasing, solution-wide.</summary>
public static class FriendlyValidationMessages
{
    public static void Apply()
    {
        var english = (LanguageManager)ValidatorOptions.Global.LanguageManager;

        // Force "en" - LanguageManager won't fall back from CurrentUICulture (e.g. "en-US") to it.
        english.Culture = new System.Globalization.CultureInfo("en");

        english.AddTranslation("en", "NotEmptyValidator", "{PropertyName} is required.");
        english.AddTranslation("en", "NotNullValidator", "{PropertyName} is required.");
        english.AddTranslation("en", "MaximumLengthValidator", "{PropertyName} must be {MaxLength} characters or fewer.");
        english.AddTranslation("en", "MinimumLengthValidator", "{PropertyName} must be at least {MinLength} characters.");
        english.AddTranslation("en", "LengthValidator", "{PropertyName} must be between {MinLength} and {MaxLength} characters.");
        english.AddTranslation("en", "ExactLengthValidator", "{PropertyName} must be exactly {MaxLength} characters.");
        english.AddTranslation("en", "GreaterThanValidator", "{PropertyName} must be greater than {ComparisonValue}.");
        english.AddTranslation("en", "GreaterThanOrEqualValidator", "{PropertyName} must be {ComparisonValue} or greater.");
        english.AddTranslation("en", "LessThanValidator", "{PropertyName} must be less than {ComparisonValue}.");
        english.AddTranslation("en", "LessThanOrEqualValidator", "{PropertyName} must be {ComparisonValue} or less.");
        english.AddTranslation("en", "EqualValidator", "{PropertyName} must equal {ComparisonValue}.");
        english.AddTranslation("en", "NotEqualValidator", "{PropertyName} must not equal {ComparisonValue}.");
        english.AddTranslation("en", "InclusiveBetweenValidator", "{PropertyName} must be between {From} and {To}.");
        english.AddTranslation("en", "ExclusiveBetweenValidator", "{PropertyName} must be between {From} and {To} (exclusive).");
        english.AddTranslation("en", "EmailValidator", "{PropertyName} must be a valid email address.");
        english.AddTranslation("en", "RegularExpressionValidator", "{PropertyName} is not in the correct format.");
        english.AddTranslation("en", "PredicateValidator", "{PropertyName} is not valid.");
        english.AddTranslation("en", "AsyncPredicateValidator", "{PropertyName} is not valid.");
    }
}
