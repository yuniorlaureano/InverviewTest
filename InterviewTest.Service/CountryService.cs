using AutoMapper;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface ICountryService
    {
        public Task<ExecutionResult<CountryListDto?>> GetByIdAsync(long id);
        Task<ExecutionResult<IEnumerable<CountryListDto>>> GetAsync(int page = 1, int pageSize = 10, string? name = null);
        public Task<ExecutionResult> AddAsync(CountryCreationDto countryDto);
        public Task<ExecutionResult> UpdateAsync(CountryUpdateDto countryDto);
    }

    public class CountryService : ICountryService
    {
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<ExecutionResult> AddAsync(CountryCreationDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            return await _countryRepository.AddAsync(country);
        }

        public async Task<ExecutionResult<CountryListDto?>> GetByIdAsync(long id)
        {
            var result = await _countryRepository.GetByIdAsync(id);
            return result.Clone(
                _mapper.Map<CountryListDto?>(result.Data)
                );
        }

        public async Task<ExecutionResult<IEnumerable<CountryListDto>>> GetAsync(int page = 1, int pageSize = 10, string? name = null)
        {
            var result = await _countryRepository.GetAsync(page, pageSize, name);
            return result.Clone(
                _mapper.Map<IEnumerable<CountryListDto>>(result.Data)
                );
        }

        public async Task<ExecutionResult> UpdateAsync(CountryUpdateDto countryDtos)
        {
            var country = _mapper.Map<Country>(countryDtos);
            return await _countryRepository.UpdateAsync(country);
        }
    }
}
