using FluentValidation;
using Web.Application.DTO_s;

namespace Web.Application.Validators.User
{
    public class AuthUserDtoValidator : AbstractValidator<AuthUserDto>
    {
        public AuthUserDtoValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
