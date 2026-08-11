namespace OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;

internal class ValidationMessage
{
    public const string TheTitleNotEmpty = "Please provide a title. This field cannot be left empty.";
    public const string TheTitleMinimumLength = "The title must contain at least 5 characters.";

    public const string TheDescriptionNotEmpty = "Please provide a description. This field cannot be left empty.";
    public const string TheDescriptionMinimumLength = "The description must contain at least 5 characters.";

    public const string TheImageNotEmpty = "Please upload an image. This field cannot be left empty.";
    public const string TheImageMustFormat = "Only the following image formats are allowed: .jpg, .png, etc.";

    public const string TheVideoUrlNotEmpty = "Please provide a video URL. This field cannot be left empty.";
    public const string TheUrlNotEmpty = "Please provide a URL. This field cannot be left empty.";
    public const string TheUrlMinimumLength = "The url must contain at least 17 characters.";
    public const string TheUrlMatches = @"^(https?:\/\/)?([a-zA-Z0-9]+[.])+[a-zA-Z]{2,}(:[0-9]{1,5})?(\/.*)?$";
    public const string TheUrlMatchesExample = "Please enter a valid URL (e.g., https://www.example.com).";

    public const string TheCoverImageNotEmpty = "Please upload a cover image. This field cannot be left empty.";

    public const string TheImgIconNotEmpty = "Please select an icon. This field is required.";
    public const string TheClassIconNotEmpty = "Please select an icon. This field is required.";

    public const string TheNameNotEmpty = "Please provide a name. This field cannot be left empty.";
    public const string TheNameMinimumLength = "The name must contain at least 5 characters.";
}
