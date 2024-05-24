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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserCreationDto> _userCreationValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;

        public UsersController(
            IUserService userService,
            IValidator<UserCreationDto> userCreationValidator,
            IValidator<UserUpdateDto> userUpdateValidator
            )
        {
            _userService = userService;
            _userCreationValidator = userCreationValidator;
            _userUpdateValidator = userUpdateValidator;
        }

        [HttpGet("{id:int}")]
        [Produces(typeof(UserListDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<UserListDto?> Get(int id)
        {
            return await _userService.GetByIdAsync(id);
        }

        [HttpGet()]
        [Produces(typeof(IEnumerable<UserListDto>))]
        public async Task<IEnumerable<UserListDto>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] byte? age = null,
            [FromQuery] string? country = null)
        {
            return await _userService.GetAsync(page, pageSize, age, country);
        }

        [HttpPost()]
        [AllowAnonymous]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(UserCreationDto user)
        {
            var validationResult = await _userCreationValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while creating user",
                        detail: "Error while creating user",
                        instance: HttpContext?.Request?.Path
                    );
            }

            await _userService.AddAsync(user);
            return NoContent();
        }

        [HttpPost("bulk")]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post(IEnumerable<UserCreationDto> users)
        {
            foreach (var user in users)
            {
                var validationResult = await _userCreationValidator.ValidateAsync(user);
                if (!validationResult.IsValid)
                {
                    return validationResult.FluentValidationProblem(
                            status: 400,
                            title: "Error while creating users",
                            detail: "Error while creating users",
                            instance: HttpContext?.Request?.Path
                        );
                }
            }

            await _userService.AddAsync(users);
            return NoContent();
        }

        [HttpPut()]
        [Produces(typeof(ErrorResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(UserUpdateDto user)
        {
            var validationResult = await _userUpdateValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while updating user",
                        detail: "Error while updating user",
                        instance: HttpContext?.Request?.Path
                    );
            }

            await _userService.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
