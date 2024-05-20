using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateValidator(IUserService userService)
        {
            RuleFor(x => x.FirstName)
               .NotNull()
               .NotEmpty()
               .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .MustAsync(async (email, token) =>
                {
                    return (await userService.GetByEmail(email)) is null;
                });

            RuleFor(x => x.Age).Must((age) => age > 0 && age <= byte.MaxValue)
                .WithMessage($"Must provide an age between 1 and {byte.MaxValue}");
        }
    }
}
