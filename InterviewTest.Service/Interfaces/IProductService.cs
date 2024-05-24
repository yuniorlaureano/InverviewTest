using InterviewTest.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewTest.Service.Interfaces
{
    public interface IProductService
    {
        public Task<ProductListDto?> GetByIdAsync(long id);
        Task<IEnumerable<ProductListDto>> GetByIdsAsync(List<long> ids);
        Task<IEnumerable<ProductListDto>> GetAsync(int page = 1, int pageSize = 10, string? code = null, string? name = null);
        public Task AddAsync(ProductCreationDto productDto);
        public Task UpdateAsync(ProductUpdateDto productDto);
        public Task DeleteAsync(long id);
    }
}
