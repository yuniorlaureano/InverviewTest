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
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _provinceService;
        private readonly IValidator<ProvinceCreationDto> _provinceCreationValidator;
        private readonly IValidator<ProvinceUpdateDto> _provinceUpdateValidator;

        public ProvincesController(
            IProvinceService provinceService,
            IValidator<ProvinceCreationDto> provinceCreationValidator,
            IValidator<ProvinceUpdateDto> provinceUpdateValidator
            )
        {
            _provinceService = provinceService;
            _provinceCreationValidator = provinceCreationValidator;
            _provinceUpdateValidator = provinceUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(ProvinceListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ProvinceListDto?> Get(int id)
        {
            return await _provinceService.GetById(id);
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<ProvinceListDto>))]
        public async Task<IEnumerable<ProvinceListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? country = null)
        {
            return await _provinceService.Get(page, pageSize, name, country);
        }

        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(ProvinceCreationDto province)
        {
            var validationResult = await _provinceCreationValidator.ValidateAsync(province);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating province",
                        detail: "Error while creating province",
                        instance: HttpContext.Request.Path
                    );
            }

            await _provinceService.Add(province);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(ProvinceUpdateDto province)
        {
            var validationResult = await _provinceUpdateValidator.ValidateAsync(province);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating province",
                        detail: "Error while updating province",
                        instance: HttpContext.Request.Path
                    );
            }

            await _provinceService.Update(province);
            return NoContent();
        }
    }
}
