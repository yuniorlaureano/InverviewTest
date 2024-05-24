using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.AspNetCore.Mvc;
using InterviewTest.Api.Util;
using Microsoft.AspNetCore.Authorization;
using InterviewTest.Common;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IValidator<StockCreationDto> _stockCreationValidator;
        private readonly IValidator<StockUpdateDto> _stockUpdateValidator;

        public StocksController(
            IStockService stockService,
            IValidator<StockCreationDto> stockCreationValidator,
            IValidator<StockUpdateDto> stockUpdateValidator
            )
        {
            _stockService = stockService;
            _stockCreationValidator = stockCreationValidator;
            _stockUpdateValidator = stockUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(StockListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<StockListDto?> Get(int id)
        {
            return await _stockService.GetByIdAsync(id);
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<StockListDto>))]
        public async Task<IEnumerable<StockListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            TransactionType? transactionType = null,
            string? description = null)
        {
            return await _stockService.GetAsync(page, pageSize, transactionType, description);
        }

        [HttpGet("available-products")]
        [Produces(typeof(IEnumerable<AvailableProductDto>))]
        public async Task<IEnumerable<AvailableProductDto>> GetProductsInStock(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           TransactionType? transactionType = null)
        {
            return await _stockService.GetProductsInStockAsync(page, pageSize, transactionType);
        }


        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(StockCreationDto stock)
        {
            var validationResult = await _stockCreationValidator.ValidateAsync(stock);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating stock",
                        detail: "Error while creating stock",
                        instance: HttpContext.Request.Path
                    );
            }

            await _stockService.AddAsync(stock);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(StockUpdateDto stock)
        {
            var validationResult = await _stockUpdateValidator.ValidateAsync(stock);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating stock",
                        detail: "Error while updating stock",
                        instance: HttpContext.Request.Path
                    );
            }

            await _stockService.UpdateAsync(stock);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            await _stockService.DeleteAsync(id);
            return NoContent();
        }
    }
}
