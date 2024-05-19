﻿using FluentValidation;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Code)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Price).Must((price) => price > 0 && price <= decimal.MaxValue)
                .WithMessage($"Must provide a valid price");
        }
    }
}
