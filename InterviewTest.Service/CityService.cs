using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface ICityService
    {
        public Task<CityListDto?> GetById(long id);
        Task<IEnumerable<CityListDto>> Get(int page = 1, int pageSize = 10, string? name = null, string? City = null);
        public Task Add(CityCreationDto CityDto);
        public Task Update(CityUpdateDto CityDto);
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

        public async Task Add(CityCreationDto cityDto)
        {
            var city = _mapper.Map<City>(cityDto);
            await _CityRepository.Add(city);
        }

        public async Task<CityListDto?> GetById(long id)
        {
            var city = await _CityRepository.GetById(id);
            return _mapper.Map<CityListDto>(city);
        }

        public async Task<IEnumerable<CityListDto>> Get(int page = 1, int pageSize = 10, string? name = null, string? city = null)
        {
            var citys = await _CityRepository.Get(page, pageSize, name, city);
            return _mapper.Map<IEnumerable<CityListDto>>(citys);
        }

        public async Task Update(CityUpdateDto cityDto)
        {
            var city = _mapper.Map<City>(cityDto);
            await _CityRepository.Update(city);
        }
    }
}
