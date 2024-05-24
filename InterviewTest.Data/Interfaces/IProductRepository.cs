using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product?> GetByIdAsync(long id);
        Task<IEnumerable<Product>> GetByIdsAsync(List<long> ids);
        public Task<IEnumerable<Product>> GetAsync(int page = 1, int pageSize = 10, string? code = null, string? name = null);
        public Task AddAsync(Product product);
        public Task UpdateAsync(Product product);
        public Task DeleteAsync(long id);
    }
}
