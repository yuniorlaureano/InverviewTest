using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IProductService
    {
        public Task<ProductListDto?> GetById(long id);
        Task<IEnumerable<ProductListDto>> GetByIds(List<long> ids);
        Task<IEnumerable<ProductListDto>> Get(int page = 1, int pageSize = 10, string? code = null, string? name = null);
        public Task Add(ProductCreationDto productDto);
        public Task Update(ProductUpdateDto productDto);
        public Task Delete(long id);
    }

    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task Add(ProductCreationDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.Add(product);
        }

        public async Task Delete(long id)
        {
            await _productRepository.Delete(id);
        }

        public async Task<ProductListDto?> GetById(long id)
        {
            var product = await _productRepository.GetById(id);
            return _mapper.Map<ProductListDto>(product);
        }

        public async Task<IEnumerable<ProductListDto>> GetByIds(List<long> ids)
        {
            var products = await _productRepository.GetByIds(ids);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> Get(int page = 1, int pageSize = 10, string? code = null, string? name = null)
        {
            var products = await _productRepository.Get(page, pageSize, code, name);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task Update(ProductUpdateDto productDtos)
        {
            var product = _mapper.Map<Product>(productDtos);
            await _productRepository.Update(product);
        }
    }
}
