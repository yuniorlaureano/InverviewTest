﻿using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service.Validators
{
    public class ProvinceCreationValidator : AbstractValidator<ProvinceCreationDto>
    {
        public ProvinceCreationValidator(IProvinceService provinceService, ICountryService countryService)
        {
            RuleFor(x => x.Name)
               .NotNull()
               .NotEmpty()
               .MaximumLength(50)
               .MustAsync(async (name, token) =>
               {
                   return !(await provinceService.GetAsync(name: name)).Any();
               }).WithMessage("The province already exist");

            RuleFor(x => x.CountryId)
                .NotNull()
                .MustAsync(async (countryId, token) =>
                {
                    return (await countryService.GetByIdAsync(countryId)) is not null;
                }).WithMessage("The country does not exist");

        }
    }
}
