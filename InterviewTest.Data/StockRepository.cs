using InterviewTest.Common;
using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    //ToDo: Add serilog 
    public interface IStockRepository
    {
        public Task<Stock?> GetById(long id);
        public Task<IEnumerable<Stock>> Get(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null);
        public Task Add(Stock stock, IEnumerable<StockDetail> stockDetails);
        public Task Update(Stock stock, IEnumerable<StockDetail> stockDetails);
        public Task Delete(long id);
    }

    public class StockRepository : IStockRepository
    {
        private readonly IADOCommand _adoCommand;

        public StockRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task Add(Stock stock, IEnumerable<StockDetail> stockDetails)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                command.CommandText = @$"INSERT INTO [{nameof(Stock)}]
                {_adoCommand.GenerateInsertColumnsBody(
                    nameof(Stock.TransactionType),
                    nameof(Stock.Description),
                    nameof(Stock.Date)
                )};
                SELECT SCOPE_IDENTITY();
                ";

                AddParamenters(command, stock);
                var stockId = await command.ExecuteScalarAsync();

                if (stockId == null)
                {
                    throw new InvalidOperationException("The id of the stock is null");
                }

                command.Parameters.Clear();
                command.CommandText = @$"INSERT INTO [{nameof(StockDetail)}]
                {_adoCommand.GenerateInsertColumnsBody(
                    nameof(StockDetail.Quantity),
                    nameof(StockDetail.ProductId),
                    nameof(StockDetail.StockId)
                )}";

                var quantityParameter = new SqlParameter(nameof(StockDetail.Quantity), SqlDbType.Int);
                var productIdParameter = new SqlParameter(nameof(StockDetail.ProductId), SqlDbType.BigInt);
                var stockIdParameter = new SqlParameter(nameof(StockDetail.StockId), SqlDbType.BigInt);

                command.Parameters.AddRange(new[]
                {
                    quantityParameter,
                    productIdParameter,
                    stockIdParameter
                });

                stockIdParameter.Value = Convert.ToInt64(stockId);
                foreach (var stockDetail in stockDetails)
                {
                    quantityParameter.Value = stockDetail.Quantity;
                    productIdParameter.Value = stockDetail.ProductId;
                    onQuery();
                    await command.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task Delete(long id)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(Stock.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"DELETE FROM [{nameof(Stock)}] WHERE Id = @Id";
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<Stock?> GetById(long id)
        {
            Stock? stock = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(stock.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(Stock)}] WHERE {nameof(stock.Id)} = @{nameof(stock.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stock = MapStockFromReader(reader);
                }
            });

            return stock;
        }

        public async Task<IEnumerable<Stock>> Get(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null)
        {
            var stocks = new List<Stock>();
            await _adoCommand.Execute(async (command) =>
            {
                var filter = _adoCommand.CreateFilter(command,
                   new SqlFilterParam(nameof(Stock.Description), description, SqlDbType.NVarChar),
                   new SqlFilterParam(nameof(Stock.TransactionType), transactionType, SqlDbType.TinyInt)
                );

                var paging = _adoCommand.CreatePaging(page, pageSize);

                command.CommandText = $"SELECT * FROM  [{nameof(Stock)}] {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    stocks.Add(MapStockFromReader(reader));
                }
            });

            return stocks;
        }

        public async Task Update(Stock stock, IEnumerable<StockDetail> stockDetails)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                //Update the stock master
                command.CommandText = @$"UPDATE [{nameof(Stock)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(Stock.TransactionType),
                           nameof(Stock.Description),
                           nameof(Stock.Date)
                      )}                    
                   WHERE {nameof(Stock.Id)} = @{nameof(Stock.Id)}";

                AddParamenters(command, stock);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(stock.Id), stock.Id, SqlDbType.BigInt));
                await command.ExecuteNonQueryAsync();

                //Remove the stored stock detail
                command.Parameters.Clear();
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(Stock.Id)}", stock.Id, SqlDbType.BigInt));
                command.CommandText = $"DELETE FROM [{nameof(StockDetail)}] WHERE StockId = @Id";
                await command.ExecuteNonQueryAsync();

                //Add the new stock detail
                command.Parameters.Clear();
                command.CommandText = @$"INSERT INTO [{nameof(StockDetail)}]
                {_adoCommand.GenerateInsertColumnsBody(
                    nameof(StockDetail.Quantity),
                    nameof(StockDetail.ProductId),
                    nameof(StockDetail.StockId)
                )}";

                var quantityParameter = new SqlParameter(nameof(StockDetail.Quantity), SqlDbType.Int);
                var productIdParameter = new SqlParameter(nameof(StockDetail.ProductId), SqlDbType.BigInt);
                var stockIdParameter = new SqlParameter(nameof(StockDetail.StockId), SqlDbType.BigInt);

                command.Parameters.AddRange(new[]
                {
                    quantityParameter,
                    productIdParameter,
                    stockIdParameter
                });

                stockIdParameter.Value = stock.Id;
                foreach (var stockDetail in stockDetails)
                {
                    quantityParameter.Value = stockDetail.Quantity;
                    productIdParameter.Value = stockDetail.ProductId;
                    onQuery();
                    await command.ExecuteNonQueryAsync();
                }
            });
        }

        private void AddParamenters(SqlCommand command, Stock stock)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Stock.TransactionType), (byte)stock.TransactionType, SqlDbType.TinyInt));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Stock.Description), stock.Description, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Stock.Date), stock.Date, SqlDbType.DateTime));
        }

        private Stock MapStockFromReader(SqlDataReader reader)
        {
            return new Stock
            {
                Id = reader.GetInt64(nameof(Stock.Id)),
                TransactionType = (TransactionType)reader.GetByte(nameof(Stock.TransactionType)),
                Description = reader.GetString(nameof(Stock.Description)),
                Date = reader.GetDateTime(nameof(Stock.Date))
            };
        }
    }
}