using FluentValidation;
using InterviewTest.Api.Util;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly IValidator<CityCreationDto> _cityCreationValidator;
        private readonly IValidator<CityUpdateDto> _cityUpdateValidator;

        public CitiesController(
            ICityService cityService,
            IValidator<CityCreationDto> cityCreationValidator,
            IValidator<CityUpdateDto> cityUpdateValidator
            )
        {
            _cityService = cityService;
            _cityCreationValidator = cityCreationValidator;
            _cityUpdateValidator = cityUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(CityListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<CityListDto?> Get(int id)
        {
            return await _cityService.GetById(id);
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<CityListDto>))]
        public async Task<IEnumerable<CityListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? province = null)
        {
            return await _cityService.Get(page, pageSize, name, province);
        }

        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(CityCreationDto city)
        {
            var validationResult = await _cityCreationValidator.ValidateAsync(city);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating City",
                        detail: "Error while creating City",
                        instance: HttpContext.Request.Path
                    );
            }

            await _cityService.Add(city);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(CityUpdateDto city)
        {
            var validationResult = await _cityUpdateValidator.ValidateAsync(city);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating City",
                        detail: "Error while updating City",
                        instance: HttpContext.Request.Path
                    );
            }

            await _cityService.Update(city);
            return NoContent();
        }
    }
}
