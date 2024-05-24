using AutoFixture;
using InterviewTest.Common.Dto;
using InterviewTest.Test.Fixtures;

namespace InterviewTest.Test.Tests.Service
{
    [Collection(nameof(InventoryServiceTestCollection))]
    public class ProductServiceTest
    {
        private readonly InventoryServiceTestFixture _inventoryServiceTestFixture;

        public ProductServiceTest(InventoryServiceTestFixture inventoryServiceTestFixture)
        {
            _inventoryServiceTestFixture = inventoryServiceTestFixture;
        }

        [Fact]
        public async Task Should_Get_Product_By_Id()
        {
            await _inventoryServiceTestFixture.MockProduct();

            var insertedId = (await _inventoryServiceTestFixture.ProductService.GetAsync()).FirstOrDefault()?.Id;
            var insertedProduct = await _inventoryServiceTestFixture.ProductService.GetByIdAsync(insertedId ?? 0);

            Assert.NotNull(insertedProduct);
        }

        [Fact]
        public async Task Should_Get_Products()
        {
            await _inventoryServiceTestFixture.MockProduct();

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync();
            Assert.True(insertedProducts.Any());
        }

        [Fact]
        public async Task Should_Get_Products_By_Ids()
        {
            var products = await _inventoryServiceTestFixture.MockProducts();

            var productByIds =
                await _inventoryServiceTestFixture.ProductService.GetByIdsAsync(
                    products.Select(x => x.Id).ToList());

            Assert.True(productByIds.Any());
            Assert.Equal(2, productByIds.Count());
        }

        [Fact]
        public async Task Should_Get_Product_By_Code()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();
            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(code: product.Code);

            Assert.True(insertedProducts.Any());
        }

        [Fact]
        public async Task Should_Get_Product_By_Name()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(name: product.Name);

            Assert.True(insertedProducts.Any());
        }

        [Fact]
        public async Task Should_Get_Product_By_Code_And_Name()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(code: product.Code, name: product.Name);

            Assert.True(insertedProducts.Any());
        }

        [Fact]
        public async Task Should_Create_Product()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();

            var insertedProduct = await _inventoryServiceTestFixture.ProductService.GetAsync(code: product.Code);

            Assert.NotNull(insertedProduct);
            Assert.Equal(insertedProduct.First().Code, product.Code);
        }

        [Fact]
        public async Task Should_Update_Product()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(code: product.Code);
            var insertedProduct = insertedProducts.First();

            var productToUpdate = _inventoryServiceTestFixture.Mapper.Map<ProductUpdateDto>(insertedProduct);

            productToUpdate.Code = Guid.NewGuid().ToString();
            await _inventoryServiceTestFixture.ProductService.UpdateAsync(productToUpdate);

            var updatedProduct = await _inventoryServiceTestFixture.ProductService.GetByIdAsync(productToUpdate.Id);

            Assert.NotNull(updatedProduct);
            Assert.Equal(updatedProduct.Code, productToUpdate.Code);
        }

        [Fact]
        public async Task Should_Delete_Product()
        {
            var product = await _inventoryServiceTestFixture.MockProduct();

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(code: product.Code);
            var insertedProduct = insertedProducts.First();

            var beforeRemove = await _inventoryServiceTestFixture.ProductService.GetByIdAsync(insertedProduct.Id);
            await _inventoryServiceTestFixture.ProductService.DeleteAsync(insertedProduct.Id);
            var afterRemove = await _inventoryServiceTestFixture.ProductService.GetByIdAsync(insertedProduct.Id);

            Assert.NotNull(beforeRemove);
            Assert.Null(afterRemove);
        }

        [Fact]
        public async Task Should_Paginate_To_5_Products()
        {
            var products = Enumerable.Range(1, 10).Select(x =>
                _inventoryServiceTestFixture.Fixture
                 .Build<ProductCreationDto>()
                 .Create());

            foreach (var product in products)
            {
                await _inventoryServiceTestFixture.ProductService.AddAsync(product!);
            }

            var insertedProducts = await _inventoryServiceTestFixture.ProductService.GetAsync(pageSize: 5);

            Assert.True(insertedProducts.Any());
            Assert.True(insertedProducts.Count() == 5);
        }
    }
}