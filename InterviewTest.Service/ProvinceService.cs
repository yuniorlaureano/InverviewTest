using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data.Interfaces;
using InterviewTest.Entity;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service
{
    public class ProvinceService : IProvinceService
    {
        private readonly IMapper _mapper;
        private readonly IProvinceRepository _ProvinceRepository;

        public ProvinceService(IProvinceRepository ProvinceRepository, IMapper mapper)
        {
            _ProvinceRepository = ProvinceRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(ProvinceCreationDto ProvinceDto)
        {
            var Province = _mapper.Map<Province>(ProvinceDto);
            await _ProvinceRepository.AddAsync(Province);
        }

        public async Task<ProvinceListDto?> GetByIdAsync(long id)
        {
            var Province = await _ProvinceRepository.GetByIdAsync(id);
            return _mapper.Map<ProvinceListDto>(Province);
        }

        public async Task<IEnumerable<ProvinceListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null)
        {
            var provinces = await _ProvinceRepository.GetAsync(page, pageSize, name, province);
            return _mapper.Map<IEnumerable<ProvinceListDto>>(provinces);
        }

        public async Task UpdateAsync(ProvinceUpdateDto provinceDtos)
        {
            var province = _mapper.Map<Province>(provinceDtos);
            await _ProvinceRepository.UpdateAsync(province);
        }
    }
}
