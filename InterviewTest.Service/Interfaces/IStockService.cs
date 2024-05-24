using InterviewTest.Common;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Interfaces
{
    public interface IStockService
    {
        public Task<StockListDto?> GetByIdAsync(long id);
        Task<IEnumerable<StockListDto>> GetAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null);
        Task<IEnumerable<AvailableProductDto>> GetProductsInStockAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null);
        public Task AddAsync(StockCreationDto userDto);
        public Task UpdateAsync(StockUpdateDto userDto);
        public Task DeleteAsync(long id);
    }
}
