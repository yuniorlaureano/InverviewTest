using FluentValidation;
using InterviewTest.Api.Util;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IValidator<UserLoginDto> _loginValidator;

        public SecurityController(
            IJwtService jwtService,
            IUserService userService,
            IValidator<UserLoginDto> loginValidator
            )
        {
            _jwtService = jwtService;
            _userService = userService;
            _loginValidator = loginValidator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [Produces(typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult?> Login(UserLoginDto credentials)
        {
            var validationResult = await _loginValidator.ValidateAsync(credentials);
            if (!validationResult.IsValid)
            {
                return validationResult.FluentValidationProblem(
                        status: 400,
                        title: "Error while login in the user",
                        instance: HttpContext?.Request?.Path
                    );
            }

            var user = await _userService.GetByEmailAsync(credentials.Email);
            if (user == null)
            {
                return NotFound();
            }

            var isCorrectPassword = PasswordHasher.VerifyPassword(credentials.Password, user.Password);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }

            var claims = new Claim[]
            {
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Email, user.Email),
            };

            var token = _jwtService.GenerateTokenAsync(claims);
            return Ok(token);
        }
    }
}
