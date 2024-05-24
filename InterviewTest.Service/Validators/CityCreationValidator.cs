using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class CityCreationValidator : AbstractValidator<CityCreationDto>
    {
        public CityCreationValidator(ICityService cityService, IProvinceService provinceService)
        {
            RuleFor(x => x.Name)
               .NotNull()
               .NotEmpty()
               .MaximumLength(50)
               .MustAsync(async (name, token) =>
               {
                   return !(await cityService.GetAsync(name: name)).Any();
               }).WithMessage("The city already exist");

            RuleFor(x => x.ProvinceId)
                .NotNull()
                .MustAsync(async (province, token) =>
                {
                    return (await provinceService.GetByIdAsync(province)) is not null;
                }).WithMessage("The province does not exist");

        }
    }
}
