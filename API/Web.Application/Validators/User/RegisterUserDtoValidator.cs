using FluentValidation;
using Web.Application.DTO_s.User;

namespace Web.Application.Validators.User
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(100)
                .MinimumLength(10)
                .Matches("[A-Z]")
                .Matches("[a-z]")
                .Matches("[0-9]");
        }
    }
}
