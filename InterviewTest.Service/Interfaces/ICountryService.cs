using InterviewTest.Common;
using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Interfaces
{
    public interface ICountryService
    {
        public Task<ExecutionResult<CountryListDto?>> GetByIdAsync(long id);
        Task<ExecutionResult<IEnumerable<CountryListDto>>> GetAsync(int page = 1, int pageSize = 10, string? name = null);
        public Task<ExecutionResult> AddAsync(CountryCreationDto countryDto);
        public Task<ExecutionResult> UpdateAsync(CountryUpdateDto countryDto);
    }
}
