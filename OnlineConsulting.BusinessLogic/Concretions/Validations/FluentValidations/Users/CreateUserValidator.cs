using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Users;

internal class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    //ASP.NET's automatic FluentValidation pipeline only supports synchronous rules, so the
    //uniqueness checks below query the UserManager's IQueryable directly (sync) instead of
    //using the async FindByNameAsync/FindByEmailAsync lookups.
    public CreateUserValidator(UserManager<User> userManager, IdentityErrorDescriber errorDescriber, ILookupNormalizer normalizer)
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.")
            .Must(userName => !userManager.Users.Any(u => u.NormalizedUserName == normalizer.NormalizeName(userName)))
            .WithMessage(x => errorDescriber.DuplicateUserName(x.UserName).Description);

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .Must(email => !userManager.Users.Any(u => u.NormalizedEmail == normalizer.NormalizeEmail(email)))
            .WithMessage(x => errorDescriber.DuplicateEmail(x.Email).Description);

        //jQuery Unobtrusive Validation only supports a single data-val-regex per field, so
        //multiple Matches() rules on Password can't all get instant client-side validation -
        //only one would win. A single combined regex keeps live (no-JS) validation working.
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one symbol.");

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Confirm password must match.");
    }
}
