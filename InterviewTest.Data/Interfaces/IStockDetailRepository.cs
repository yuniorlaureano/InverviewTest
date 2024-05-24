using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface IStockDetailRepository
    {
        Task<IEnumerable<StockDetail>> GetByStockIdAsync(long stockId);
    }
}
