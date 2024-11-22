using FluentValidation;
using Web.Application.Dto_s.User;

namespace Web.Application.Validators.User
{
    public class UpdatePasswordDtoValidator : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MaximumLength(100)
                .MinimumLength(10)
                .Matches("[A-Z]")
                .Matches("[a-z]")
                .Matches("[0-9]");
        }
    }
}