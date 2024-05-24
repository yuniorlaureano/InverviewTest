using FluentValidation;
using InterviewTest.Common.Dto;
using Microsoft.AspNetCore.Mvc;
using InterviewTest.Api.Util;
using Microsoft.AspNetCore.Authorization;
using InterviewTest.Service.Interfaces;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductCreationDto> _productCreationValidator;
        private readonly IValidator<ProductUpdateDto> _productUpdateValidator;

        public ProductsController(
            IProductService productService,
            IValidator<ProductCreationDto> productCreationValidator,
            IValidator<ProductUpdateDto> productUpdateValidator
            )
        {
            _productService = productService;
            _productCreationValidator = productCreationValidator;
            _productUpdateValidator = productUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(ProductListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ProductListDto?> Get(int id)
        {
            return await _productService.GetByIdAsync(id);
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<ProductListDto>))]
        public async Task<IEnumerable<ProductListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? code = null,
            [FromQuery] string? name = null)
        {
            return await _productService.GetAsync(page, pageSize, code, name);
        }

        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(ProductCreationDto product)
        {
            var validationResult = await _productCreationValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating product",
                        detail: "Error while creating product",
                        instance: HttpContext.Request.Path
                    );
            }

            await _productService.AddAsync(product);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(ProductUpdateDto product)
        {
            var validationResult = await _productUpdateValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating product",
                        detail: "Error while updating product",
                        instance: HttpContext.Request.Path
                    );
            }

            await _productService.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
