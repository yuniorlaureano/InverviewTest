using AutoMapper;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data.Interfaces;
using InterviewTest.Entity;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Service
{
    public class StockService : IStockService
    {
        private readonly IMapper _mapper;
        private readonly IStockRepository _stockRepository;
        private readonly IStockDetailRepository _stockDetailRepository;

        public StockService(IStockRepository stockRepository, IStockDetailRepository stockDetailRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _stockDetailRepository = stockDetailRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(StockCreationDto stockDto)
        {
            var stock = _mapper.Map<Stock>(stockDto);
            var stockDetails = _mapper.Map<IEnumerable<StockDetail>>(stockDto.StockDetailListDtos);
            await _stockRepository.AddAsync(stock, stockDetails);
        }

        public async Task DeleteAsync(long id)
        {
            await _stockRepository.DeleteAsync(id);
        }

        public async Task<StockListDto?> GetByIdAsync(long id)
        {
            var user = await _stockRepository.GetByIdAsync(id);
            var stock = _mapper.Map<StockListDto>(user);
            if (stock is not null)
            {
                var stockListDetails = await _stockDetailRepository.GetByStockIdAsync(stock.Id);
                stock.StockDetailListDtos = _mapper.Map<IEnumerable<StockDetailListDto>>(stockListDetails);
            }
            return stock;
        }

        public async Task<IEnumerable<StockListDto>> GetAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null)
        {
            var stocks = await _stockRepository.GetAsync(page, pageSize, transactionType, description);
            var stocksDto = _mapper.Map<IEnumerable<StockListDto>>(stocks);
            foreach (var stock in stocksDto)
            {
                var stockListDetails = await _stockDetailRepository.GetByStockIdAsync(stock.Id);
                stock.StockDetailListDtos = _mapper.Map<IEnumerable<StockDetailListDto>>(stockListDetails);
            }
            return stocksDto;
        }

        public async Task<IEnumerable<AvailableProductDto>> GetProductsInStockAsync(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null)
        {
            return _mapper.Map<IEnumerable<AvailableProductDto>>(await _stockRepository.GetProductsInStockAsync(page, pageSize, transactionType));
        }

        public async Task UpdateAsync(StockUpdateDto stockDtos)
        {
            var stock = _mapper.Map<Stock>(stockDtos);
            var stockDetails = _mapper.Map<IEnumerable<StockDetail>>(stockDtos.StockDetailListDtos);
            await _stockRepository.UpdateAsync(stock, stockDetails);
        }
    }
}
