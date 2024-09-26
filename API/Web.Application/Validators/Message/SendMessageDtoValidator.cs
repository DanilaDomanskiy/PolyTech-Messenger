using FluentValidation;
using Web.Application.DTO_s.Message;

namespace Web.Application.Validators.Message
{
    public class SendMessageDtoValidator : AbstractValidator<SendMessageDto>
    {
        public SendMessageDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(5000);

            RuleFor(x => x.Timestamp)
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.ConnectionId)
                .NotEmpty();

            RuleFor(x => new { x.GroupId, x.PrivateChatId })
                .Must(x => x.GroupId.HasValue || x.PrivateChatId.HasValue);

            RuleFor(x => x.GroupId)
                .Null().When(x => x.PrivateChatId.HasValue);

            RuleFor(x => x.PrivateChatId)
                .Null().When(x => x.GroupId.HasValue);
        }
    }
}
