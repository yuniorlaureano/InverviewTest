using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service.Validators
{
    public class CityUpdateValidator : AbstractValidator<CityUpdateDto>
    {
        public CityUpdateValidator(ICityService cityService, IProvinceService provinceService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Id)
                .NotNull()
                .MustAsync(async (id, token) =>
                {
                    return (await cityService.GetByIdAsync(id)) is not null;
                }).WithMessage("The city does not exist");

            RuleFor(x => x.ProvinceId)
                .NotNull()
                .MustAsync(async (provinceId, token) =>
                {
                    return (await provinceService.GetByIdAsync(provinceId)) is not null;
                }).WithMessage("The province does not exist");

        }
    }
}
