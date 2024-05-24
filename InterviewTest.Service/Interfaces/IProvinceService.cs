using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Interfaces
{
    public interface IProvinceService
    {
        public Task<ProvinceListDto?> GetByIdAsync(long id);
        Task<IEnumerable<ProvinceListDto>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null);
        public Task AddAsync(ProvinceCreationDto ProvinceDto);
        public Task UpdateAsync(ProvinceUpdateDto ProvinceDto);
    }
}
