using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IProvinceService
    {
        public Task<ProvinceListDto?> GetById(long id);
        Task<IEnumerable<ProvinceListDto>> Get(int page = 1, int pageSize = 10, string? name = null, string? province = null);
        public Task Add(ProvinceCreationDto ProvinceDto);
        public Task Update(ProvinceUpdateDto ProvinceDto);
    }

    public class ProvinceService : IProvinceService
    {
        private readonly IMapper _mapper;
        private readonly IProvinceRepository _ProvinceRepository;

        public ProvinceService(IProvinceRepository ProvinceRepository, IMapper mapper)
        {
            _ProvinceRepository = ProvinceRepository;
            _mapper = mapper;
        }

        public async Task Add(ProvinceCreationDto ProvinceDto)
        {
            var Province = _mapper.Map<Province>(ProvinceDto);
            await _ProvinceRepository.Add(Province);
        }

        public async Task<ProvinceListDto?> GetById(long id)
        {
            var Province = await _ProvinceRepository.GetById(id);
            return _mapper.Map<ProvinceListDto>(Province);
        }

        public async Task<IEnumerable<ProvinceListDto>> Get(int page = 1, int pageSize = 10, string? name = null, string? province = null)
        {
            var provinces = await _ProvinceRepository.Get(page, pageSize, name, province);
            return _mapper.Map<IEnumerable<ProvinceListDto>>(provinces);
        }

        public async Task Update(ProvinceUpdateDto provinceDtos)
        {
            var province = _mapper.Map<Province>(provinceDtos);
            await _ProvinceRepository.Update(province);
        }
    }
}
