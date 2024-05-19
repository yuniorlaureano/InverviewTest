using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IProductRepository
    {
        public Task<Product?> GetById(long id);
        Task<IEnumerable<Product>> GetByIds(List<long> ids);
        public Task<IEnumerable<Product>> Get(int page = 1, int pageSize = 10, string? code = null, string? name = null);
        public Task Add(Product product);
        public Task Update(Product product);
        public Task Delete(long id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IADOCommand _adoCommand;

        public ProductRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task Add(Product product)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.CommandText = @$"INSERT INTO [{nameof(Product)}]
                    {_adoCommand.GenerateInsertColumnsBody(
                       nameof(Product.Code),
                       nameof(Product.Name),
                       nameof(Product.Price),
                       nameof(Product.Description)
                   )}";

                AddParamenters(command, product);
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task Delete(long id)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(Product.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"DELETE FROM [{nameof(Product)}] WHERE Id = @Id";
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<Product?> GetById(long id)
        {
            Product? product = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(product.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(Product)}] WHERE {nameof(product.Id)} = @{nameof(product.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    product = MapproductFromReader(reader);
                }
            });

            return product;
        }

        public async Task<IEnumerable<Product>> GetByIds(List<long> ids)
        {
            var products = new List<Product>();

            if (!ids.Any())
            {
                return products;
            }

            await _adoCommand.Execute(async (command) =>
            {
                var inFilter = _adoCommand.CreateInFilter(command, 
                    ids.Select(id => new SqlFilterParam(nameof(Product.Id), id, SqlDbType.BigInt)).ToArray()
                );

                command.CommandText = $"SELECT * FROM  [{nameof(Product)}] WHERE {nameof(Product.Id)} IN {inFilter}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(MapproductFromReader(reader));
                }
            });

            return products;
        }

        public async Task<IEnumerable<Product>> Get(int page = 1, int pageSize = 10, string? code = null, string? name = null)
        {
            var products = new List<Product>();
            await _adoCommand.Execute(async (command) =>
            {
                var filter = _adoCommand.CreateFilter(command,
                    new SqlFilterParam(nameof(Product.Code), code, SqlDbType.NVarChar),
                    new SqlFilterParam(nameof(Product.Name), name, SqlDbType.NVarChar)
                 );

                var paging = _adoCommand.CreatePaging(page, pageSize);

                command.CommandText = $"SELECT * FROM  [{nameof(Product)}] {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapproductFromReader(reader));
                }
            });

            return products;
        }

        public async Task Update(Product product)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.CommandText = @$"UPDATE [{nameof(Product)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(Product.Code),
                           nameof(Product.Name),
                           nameof(Product.Price),
                           nameof(Product.Description)
                      )}                    
                   WHERE {nameof(Product.Id)} = @{nameof(Product.Id)}";

                AddParamenters(command, product);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(product.Id), product.Id, SqlDbType.BigInt));
                await command.ExecuteNonQueryAsync();
            });
        }

        private void AddParamenters(SqlCommand command, Product product)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Product.Code), product.Code, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Product.Name), product.Name, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Product.Price), product.Price, SqlDbType.Decimal));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Product.Description), product.Description, SqlDbType.NVarChar));
        }

        private Product MapproductFromReader(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt64(nameof(Product.Id)),
                Code = reader.GetString(nameof(Product.Code)),
                Name = reader.GetString(nameof(Product.Name)),
                Price = reader.GetDecimal(nameof(Product.Price)),
                Description = reader.GetString(nameof(Product.Description))
            };
        }
    }
}