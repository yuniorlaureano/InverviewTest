using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;
using InterviewTest.Service;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test.Fixtures
{
    public class InventoryServiceTestFixture : IAsyncLifetime
    {
        public IServiceProvider Services { get; private set; }
        public IFixture Fixture { get; private set; }
        public IProductService ProductService { get; private set; }
        public IStockService StockService { get; private set; }
        public IMapper Mapper { get; private set; }
        public IADOCommand AdoCommand { get; private set; }

        public InventoryServiceTestFixture()
        {
            Services = DependencyBuilder.GetServices();
            Fixture = new Fixture();
            ProductService = Services.GetRequiredService<IProductService>();
            StockService = Services.GetRequiredService<IStockService>();
            Mapper = Services.GetRequiredService<IMapper>();
            AdoCommand = Services.GetRequiredService<IADOCommand>();
        }

        public async Task<ProductCreationDto> MockProduct()
        {
            var product = Fixture
                .Build<ProductCreationDto>()
                .Create();

            await ProductService.AddAsync(product);

            return product;
        }

        public async Task<IEnumerable<ProductListDto>> MockProducts()
        {
            var stock = Fixture
                .Build<StockCreationDto>()
                .Create();

            var product1 = Fixture
                   .Build<ProductCreationDto>()
                   .Create();

            var product2 = Fixture
                   .Build<ProductCreationDto>()
                   .Create();

            await ProductService.AddAsync(product1);
            await ProductService.AddAsync(product2);

            return await ProductService.GetAsync(pageSize: 2);
        }

        public async Task<(StockCreationDto Stock, List<StockDetailCreationDto> StockDetail)> MockStockAndDetail()
        {
            var stock = Fixture
                .Build<StockCreationDto>()
                .Create();

            var insertedProducts = await MockProducts();

            var stockDetailCreationDtos = new List<StockDetailCreationDto>();
            foreach (var product in insertedProducts)
            {
                stockDetailCreationDtos.Add(
                    Fixture
                       .Build<StockDetailCreationDto>()
                       .With(x => x.ProductId, product.Id)
                       .Create()
                );
            }

            return (stock, stockDetailCreationDtos);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }

    [CollectionDefinition(nameof(InventoryServiceTestCollection))]
    public class InventoryServiceTestCollection : ICollectionFixture<InventoryServiceTestFixture>
    {
    }
}
