using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface IProvinceRepository
    {
        public Task<ProvinceDetail?> GetByIdAsync(long id);
        public Task<IEnumerable<ProvinceDetail>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null);
        public Task AddAsync(Province Province);
        public Task UpdateAsync(Province Province);
    }
}
