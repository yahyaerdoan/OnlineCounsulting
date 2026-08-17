namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Message;

public record MessageListItemViewModel(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate);
