using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data.Interfaces;
using InterviewTest.Entity;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(ProductCreationDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
        }

        public async Task DeleteAsync(long id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<ProductListDto?> GetByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductListDto>(product);
        }

        public async Task<IEnumerable<ProductListDto>> GetByIdsAsync(List<long> ids)
        {
            var products = await _productRepository.GetByIdsAsync(ids);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> GetAsync(int page = 1, int pageSize = 10, string? code = null, string? name = null)
        {
            var products = await _productRepository.GetAsync(page, pageSize, code, name);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task UpdateAsync(ProductUpdateDto productDtos)
        {
            var product = _mapper.Map<Product>(productDtos);
            await _productRepository.UpdateAsync(product);
        }
    }
}
