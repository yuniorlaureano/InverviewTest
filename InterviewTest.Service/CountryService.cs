using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface ICountryService
    {
        public Task<CountryListDto?> GetById(long id);
        Task<IEnumerable<CountryListDto>> Get(int page = 1, int pageSize = 10, string? name = null);
        public Task Add(CountryCreationDto countryDto);
        public Task Update(CountryUpdateDto countryDto);
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

        public async Task Add(CountryCreationDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.Add(country);
        }

        public async Task<CountryListDto?> GetById(long id)
        {
            var country = await _countryRepository.GetById(id);
            return _mapper.Map<CountryListDto>(country);
        }

        public async Task<IEnumerable<CountryListDto>> Get(int page = 1, int pageSize = 10, string? name = null)
        {
            var countrys = await _countryRepository.Get(page, pageSize, name);
            return _mapper.Map<IEnumerable<CountryListDto>>(countrys);
        }

        public async Task Update(CountryUpdateDto countryDtos)
        {
            var country = _mapper.Map<Country>(countryDtos);
            await _countryRepository.Update(country);
        }
    }
}
