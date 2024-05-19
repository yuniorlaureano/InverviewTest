using AutoFixture;
using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
{
    public class StockValidationTest
    {
        private readonly IServiceProvider _services;
        private readonly IFixture _fixture;
        private readonly IValidator<StockCreationDto> _stockCreationValidator;
        private readonly IValidator<StockUpdateDto> _stockUpdateValidator;
        private readonly IProductService _productService;

        public StockValidationTest()
        {
            _services = DependencyBuilder.GetServices();
            _fixture = new Fixture();

            _stockCreationValidator = _services.GetRequiredService<IValidator<StockCreationDto>>();
            _stockUpdateValidator = _services.GetRequiredService<IValidator<StockUpdateDto>>();
            _productService = _services.GetRequiredService<IProductService>();
        }

        [Fact]
        public async Task Should_Return_Invalid_Sock_Creation_For_Empty_StockDetail()
        {
            var stock = _fixture
                .Build<StockCreationDto>()
                .With(x => x.StockDetailListDtos, Enumerable.Empty<StockDetailCreationDto>)
                .Create();

            var result = await _stockCreationValidator.ValidateAsync(stock);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Should_Return_Invalid_Stock_Update_For_Empty_StockDetail()
        {
            var stock = _fixture
                .Build<StockUpdateDto>()
                .With(x => x.StockDetailListDtos, Enumerable.Empty<StockDetailCreationDto>)
                .Create();

            var result = await _stockUpdateValidator.ValidateAsync(stock);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Should_Return_Valid_Stock_Creation_For_With_StockDetail()
        {
            var product = await MockProduct();
            var stock = _fixture
                .Build<StockCreationDto>()
                .With(x => x.StockDetailListDtos, new List<StockDetailCreationDto>
                {
                    new StockDetailCreationDto
                    {
                         ProductId = product.Id,
                         Quantity = 3
                    }
                })
                .Create();

            var result = await _stockCreationValidator.ValidateAsync(stock);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Should_Return_Valid_Stock_Update_For_With_StockDetail()
        {
            var product = await MockProduct();

            var stock = _fixture
                .Build<StockUpdateDto>()
                .With(x => x.StockDetailListDtos, new List<StockDetailCreationDto>
                {
                    new StockDetailCreationDto
                    {
                         ProductId = product.Id,
                         Quantity = 3
                    }
                })
                .Create();

            var result = await _stockUpdateValidator.ValidateAsync(stock);

            Assert.True(result.IsValid);
        }

        private async Task<ProductListDto> MockProduct()
        {
            var product = _fixture
                .Build<ProductCreationDto>()
                .Create();

            await _productService.Add(product);
            return (await _productService.Get(name: product.Name)).First();
        }
    }
}
