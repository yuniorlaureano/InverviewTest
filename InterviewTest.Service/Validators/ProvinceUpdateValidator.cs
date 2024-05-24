using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service.Validators
{
    public class ProvinceUpdateValidator : AbstractValidator<ProvinceUpdateDto>
    {
        public ProvinceUpdateValidator(IProvinceService provinceService, ICountryService countryService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Id)
                .NotNull()
                .MustAsync(async (id, token) =>
                {
                    return (await provinceService.GetByIdAsync(id)) is not null;
                }).WithMessage("The province does not exist");

            RuleFor(x => x.CountryId)
                .NotNull()
                .MustAsync(async (countryId, token) =>
                {
                    return (await countryService.GetByIdAsync(countryId)) is not null;
                }).WithMessage("The country does not exist");

        }
    }
}
