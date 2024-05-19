using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InterviewTest.Data
{
    //ToDo: Add serilog 
    public interface IStockDetailRepository
    {
        Task<IEnumerable<StockDetail>> GetByStockId(long stockId);
    }

    public class StockDetailRepository : IStockDetailRepository
    {
        private readonly IADOCommand _adoCommand;

        public StockDetailRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task<IEnumerable<StockDetail>> GetByStockId(long stockId)
        {
            var stockDetails = new List<StockDetail>();
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(StockDetail.StockId)}", stockId, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(StockDetail)}] WHERE {nameof(StockDetail.StockId)} = @{nameof(StockDetail.StockId)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stockDetails.Add(MapStockDetailFromReader(reader));
                }
            });

            return stockDetails;
        }

        private StockDetail MapStockDetailFromReader(SqlDataReader reader)
        {
            return new StockDetail
            {
                Id = reader.GetInt64(nameof(StockDetail.Id)),
                Quantity = reader.GetInt32(nameof(StockDetail.Quantity)),
                ProductId = reader.GetInt64(nameof(StockDetail.ProductId)),
                StockId = reader.GetInt64(nameof(StockDetail.StockId))
            };
        }
    }
}