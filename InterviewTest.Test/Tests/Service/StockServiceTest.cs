using AutoFixture;
using InterviewTest.Common.Dto;
using InterviewTest.Test.Fixtures;

namespace InterviewTest.Test.Tests.Service
{
    [Collection(nameof(InventoryServiceTestCollection))]
    public class StockServiceTest
    {
        private readonly InventoryServiceTestFixture _inventoryServiceTestFixture;

        public StockServiceTest(InventoryServiceTestFixture inventoryServiceTestFixture)
        {
            _inventoryServiceTestFixture = inventoryServiceTestFixture;
        }

        [Fact]
        public async Task Should_Get_Strock_By_Id()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync(description: Stock.Description);
            var insertedStock = await _inventoryServiceTestFixture.StockService.GetByIdAsync(insertedStocks.First().Id);

            Assert.NotNull(insertedStock);
        }

        [Fact]
        public async Task Should_Get_Stocks()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync();

            Assert.True(insertedStocks.Any());
        }

        [Fact]
        public async Task Should_Create_Stock()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync(description: Stock.Description);

            Assert.Single(insertedStocks);
        }

        [Fact]
        public async Task Should_Update_Stock()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync();
            var insertedStock = insertedStocks.First();

            Stock.StockDetailListDtos = StockDetail;
            insertedStock.Description = "modified";
            var newStock = _inventoryServiceTestFixture.Mapper.Map<StockUpdateDto>(insertedStock);
            await _inventoryServiceTestFixture.StockService.UpdateAsync(newStock);

            var modifiedStock = await _inventoryServiceTestFixture.StockService.GetByIdAsync(insertedStock.Id);

            Assert.Equal(insertedStock.Description, modifiedStock.Description);
        }

        [Fact]
        public async Task Should_Delete_Stock()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync();
            var insertedStock = insertedStocks.First();

            await _inventoryServiceTestFixture.StockService.DeleteAsync(insertedStock.Id);

            var deletedStock = await _inventoryServiceTestFixture.StockService.GetByIdAsync(insertedStock.Id);

            Assert.Null(deletedStock);
        }

        [Fact]
        public async Task Should_Paginate_To_5_tocks()
        {
            var insertedProducts = await _inventoryServiceTestFixture.MockProducts();

            for (int i = 0; i < 7; i++)
            {
                var stock = _inventoryServiceTestFixture.Fixture
                   .Build<StockCreationDto>()
                   .Create();

                var stockDetailCreationDtos = new List<StockDetailCreationDto>();
                foreach (var product in insertedProducts)
                {
                    stockDetailCreationDtos.Add(
                        _inventoryServiceTestFixture.Fixture
                           .Build<StockDetailCreationDto>()
                           .With(x => x.ProductId, product.Id)
                           .Create()
                    );
                }

                stock.StockDetailListDtos = stockDetailCreationDtos;
                await _inventoryServiceTestFixture.StockService.AddAsync(stock);
            }

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync(pageSize: 5);

            Assert.Equal(5, insertedStocks.Count());
        }

        [Fact]
        public async Task Should_Stock_With_2_StockDetail()
        {
            var (Stock, StockDetail) = await _inventoryServiceTestFixture.MockStockAndDetail();

            Stock.StockDetailListDtos = StockDetail;
            await _inventoryServiceTestFixture.StockService.AddAsync(Stock);

            var insertedStocks = await _inventoryServiceTestFixture.StockService.GetAsync(description: Stock.Description);

            Assert.Equal(2, insertedStocks.First().StockDetailListDtos.Count());
        }

    }
}