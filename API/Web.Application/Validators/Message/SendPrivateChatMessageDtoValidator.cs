using FluentValidation;
using Web.Application.DTO_s.Message;

namespace Web.Application.Validators.Message
{
    public class SendPrivateChatMessageDtoValidator : AbstractValidator<SendPrivateChatMessageDto>
    {
        public SendPrivateChatMessageDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(5000);

            RuleFor(x => x.Timestamp)
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.PrivateChatId)
                .NotEmpty();
        }
    }
}