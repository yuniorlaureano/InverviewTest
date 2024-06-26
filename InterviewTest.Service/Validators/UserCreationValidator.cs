﻿using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class UserCreationValidator : AbstractValidator<UserCreationDto>
    {
        public UserCreationValidator(IUserService userService)
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
                }).WithMessage("The email already exists");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Age).Must((age) => age > 0 && age <= byte.MaxValue)
                .WithMessage($"Must provide an age between 1 and {byte.MaxValue}");
        }
    }
}
