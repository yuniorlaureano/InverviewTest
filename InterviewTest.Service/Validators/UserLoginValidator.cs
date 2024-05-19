using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MaximumLength(60);
        }
    }
}
