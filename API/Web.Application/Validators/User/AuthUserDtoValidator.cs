using FluentValidation;
using Web.Application.DTO_s.User;

namespace Web.Application.Validators.User
{
    public class AuthUserDtoValidator : AbstractValidator<AuthUserDto>
    {
        public AuthUserDtoValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
