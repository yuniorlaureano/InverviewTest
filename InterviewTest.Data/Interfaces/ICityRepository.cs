using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface ICityRepository
    {
        public Task<CityDetail?> GetByIdAsync(long id);
        public Task<IEnumerable<CityDetail>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null);
        public Task AddAsync(City City);
        public Task UpdateAsync(City City);
    }
}
