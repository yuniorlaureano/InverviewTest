using FluentValidation;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.AspNetCore.Mvc;
using InterviewTest.Api.Util;
using Microsoft.AspNetCore.Authorization;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IValidator<CountryCreationDto> _CountryCreationValidator;
        private readonly IValidator<CountryUpdateDto> _CountryUpdateValidator;

        public CountriesController(
            ICountryService CountryService,
            IValidator<CountryCreationDto> CountryCreationValidator,
            IValidator<CountryUpdateDto> CountryUpdateValidator
            )
        {
            _countryService = CountryService;
            _CountryCreationValidator = CountryCreationValidator;
            _CountryUpdateValidator = CountryUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(CountryListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<CountryListDto?> Get(int id)
        {
            var result = await _countryService.GetByIdAsync(id);
            return result.Data;
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<CountryListDto>))]
        public async Task<IEnumerable<CountryListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null)
        {
            var result = await _countryService.GetAsync(page, pageSize, name);
            return result.Data;
        }

        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(CountryCreationDto country)
        {
            var validationResult = await _CountryCreationValidator.ValidateAsync(country);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating Country",
                        detail: "Error while creating Country",
                        instance: HttpContext.Request.Path
                    );
            }

            await _countryService.AddAsync(country);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(CountryUpdateDto country)
        {
            var validationResult = await _CountryUpdateValidator.ValidateAsync(country);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating Country",
                        detail: "Error while updating Country",
                        instance: HttpContext.Request.Path
                    );
            }

            await _countryService.UpdateAsync(country);
            return NoContent();
        }
    }
}
