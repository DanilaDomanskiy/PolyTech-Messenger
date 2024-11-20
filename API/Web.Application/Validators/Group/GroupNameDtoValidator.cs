using FluentValidation;
using Web.Application.Dto_s.Group;

namespace Web.Application.Validators.Group
{
    public class GroupNameDtoValidator : AbstractValidator<GroupNameDto>
    {
        public GroupNameDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(30);
        }
    }
}