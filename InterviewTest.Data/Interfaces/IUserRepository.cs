using InterviewTest.Entity;

namespace InterviewTest.Data.Interfaces
{
    public interface IUserRepository
    {
        public Task<TemporalUser?> GetByIdAsync(long id);
        Task<TemporalUser?> GetByEmailAsync(string email);
        public Task<IEnumerable<TemporalUser>> GetAsync(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
        public Task AddAsync(TemporalUser user);
        public Task AddAsync(IEnumerable<TemporalUser> user);
        public Task UpdateAsync(TemporalUser user);
        public Task DeleteAsync(long id);
    }
}
