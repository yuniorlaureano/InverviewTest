using AutoMapper;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IStockService
    {
        public Task<StockListDto?> GetById(long id);
        Task<IEnumerable<StockListDto>> Get(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null);
        Task<IEnumerable<AvailableProductDto>> GetProductsInStock(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null);
        public Task Add(StockCreationDto userDto);
        public Task Update(StockUpdateDto userDto);
        public Task Delete(long id);
    }

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

        public async Task Add(StockCreationDto stockDto)
        {
            var stock = _mapper.Map<Stock>(stockDto);
            var stockDetails = _mapper.Map<IEnumerable<StockDetail>>(stockDto.StockDetailListDtos);
            await _stockRepository.Add(stock, stockDetails);
        }

        public async Task Delete(long id)
        {
            await _stockRepository.Delete(id);
        }

        public async Task<StockListDto?> GetById(long id)
        {
            var user = await _stockRepository.GetById(id);
            var stock = _mapper.Map<StockListDto>(user);
            if (stock is not null)
            {
                var stockListDetails = await _stockDetailRepository.GetByStockId(stock.Id);
                stock.StockDetailListDtos = _mapper.Map<IEnumerable<StockDetailListDto>>(stockListDetails);
            }
            return stock;
        }

        public async Task<IEnumerable<StockListDto>> Get(
            int page = 1,
            int pageSize = 10, 
            TransactionType? transactionType = null,
            string? description = null)
        {
            var stocks = await _stockRepository.Get(page, pageSize, transactionType, description);
            var stocksDto  = _mapper.Map<IEnumerable<StockListDto>>(stocks);
            foreach (var stock in stocksDto)
            {
                var stockListDetails = await _stockDetailRepository.GetByStockId(stock.Id);
                stock.StockDetailListDtos = _mapper.Map<IEnumerable<StockDetailListDto>>(stockListDetails);
            }
            return stocksDto;
        }

        public async Task<IEnumerable<AvailableProductDto>> GetProductsInStock(
            int page = 1,
            int pageSize = 10,
            TransactionType? transactionType = null)
        {
            return _mapper.Map<IEnumerable<AvailableProductDto>>(await _stockRepository.GetProductsInStock(page, pageSize, transactionType));
        }

        public async Task Update(StockUpdateDto stockDtos)
        {
            var stock = _mapper.Map<Stock>(stockDtos);
            var stockDetails = _mapper.Map<IEnumerable<StockDetail>>(stockDtos.StockDetailListDtos);
            await _stockRepository.Update(stock, stockDetails);
        }
    }
}
