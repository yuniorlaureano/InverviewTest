using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface ICityService
    {
        public Task<CityListDto?> GetByIdAsync(long id);
        Task<IEnumerable<CityListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? City = null);
        public Task AddAsync(CityCreationDto CityDto);
        public Task UpdateAsync(CityUpdateDto CityDto);
    }

    public class CityService : ICityService
    {
        private readonly IMapper _mapper;
        private readonly ICityRepository _CityRepository;

        public CityService(ICityRepository CityRepository, IMapper mapper)
        {
            _CityRepository = CityRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(CityCreationDto cityDto)
        {
            var city = _mapper.Map<City>(cityDto);
            await _CityRepository.AddAsync(city);
        }

        public async Task<CityListDto?> GetByIdAsync(long id)
        {
            var city = await _CityRepository.GetByIdAsync(id);
            return _mapper.Map<CityListDto>(city);
        }

        public async Task<IEnumerable<CityListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? city = null)
        {
            var citys = await _CityRepository.GetAsync(page, pageSize, name, city);
            return _mapper.Map<IEnumerable<CityListDto>>(citys);
        }

        public async Task UpdateAsync(CityUpdateDto cityDto)
        {
            var city = _mapper.Map<City>(cityDto);
            await _CityRepository.UpdateAsync(city);
        }
    }
}
