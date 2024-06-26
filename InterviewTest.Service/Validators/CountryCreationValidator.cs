﻿using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class CountryCreationValidator : AbstractValidator<CountryCreationDto>
    {
        public CountryCreationValidator(ICountryService countryService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (name, token) =>
                {
                    return !(await countryService.Get(name: name)).Any();
                }).WithMessage("The country already exist");
        }
    }
}
