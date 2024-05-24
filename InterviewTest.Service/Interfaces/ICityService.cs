using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Interfaces
{
    public interface ICityService
    {
        public Task<CityListDto?> GetByIdAsync(long id);
        Task<IEnumerable<CityListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? City = null);
        public Task AddAsync(CityCreationDto CityDto);
        public Task UpdateAsync(CityUpdateDto CityDto);
    }
}
