using InterviewTest.Common;
using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface IStockRepository
    {
        public Task<Stock?> GetByIdAsync(long id);
        public Task<IEnumerable<Stock>> GetAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null);
        Task<IEnumerable<AvailableProduct>> GetProductsInStockAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null);
        public Task AddAsync(Stock stock, IEnumerable<StockDetail> stockDetails);
        public Task UpdateAsync(Stock stock, IEnumerable<StockDetail> stockDetails);
        public Task DeleteAsync(long id);
    }
}
