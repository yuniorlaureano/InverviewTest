using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface ICountryService
    {
        public Task<CountryListDto?> GetByIdAsync(long id);
        Task<IEnumerable<CountryListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null);
        public Task AddAsync(CountryCreationDto countryDto);
        public Task UpdateAsync(CountryUpdateDto countryDto);
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

        public async Task AddAsync(CountryCreationDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.AddAsync(country);
        }

        public async Task<CountryListDto?> GetByIdAsync(long id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            return _mapper.Map<CountryListDto>(country);
        }

        public async Task<IEnumerable<CountryListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null)
        {
            var countrys = await _countryRepository.GetAsync(page, pageSize, name);
            return _mapper.Map<IEnumerable<CountryListDto>>(countrys);
        }

        public async Task UpdateAsync(CountryUpdateDto countryDtos)
        {
            var country = _mapper.Map<Country>(countryDtos);
            await _countryRepository.UpdateAsync(country);
        }
    }
}
