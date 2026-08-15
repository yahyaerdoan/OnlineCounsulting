using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMessages;

public class GetMessagesValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesValidator()
    {
        RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
