using FluentValidation;
using InterviewTest.Common;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Validators
{
    public class StockCreationValidator : AbstractValidator<StockCreationDto>
    {
        public StockCreationValidator(IProductService productService)
        {
            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.TransactionType)
                .Must((type) => Enum.TryParse(typeof(TransactionType), type.ToString(), out var value))
                .WithMessage($"Allowed values: {(int)TransactionType.Input} for {nameof(TransactionType.Input)} and " +
                    $"{(int)TransactionType.Output} for {nameof(TransactionType.Output)}");

            RuleFor(x => x.StockDetailListDtos).Must((detail) => detail.Any())
                .WithMessage($"The list of product is empty");

            RuleFor(x => x.StockDetailListDtos).Must((detail) => detail.All(x => x.Quantity > 0))
                .WithMessage($"The quantity should be greater that 0");

            RuleFor(x => x.StockDetailListDtos).MustAsync(async (detail, _) =>
            {
                var products = await productService.GetByIds(detail.Select(x => x.ProductId).ToList());
                return products.Count() == detail.Count();
            }).WithMessage($"Invalid products, make sure to add existing products");
        }
    }
}
