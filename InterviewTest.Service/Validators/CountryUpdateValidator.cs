﻿using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class CountryUpdateValidator : AbstractValidator<CountryUpdateDto>
    {
        public CountryUpdateValidator(ICountryService countryService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Id)
                .NotNull()
                .MustAsync(async (id, token) =>
                {
                    return (await countryService.GetById(id)) is not null;
                }).WithMessage("The country does not exist");

        }
    }
}
