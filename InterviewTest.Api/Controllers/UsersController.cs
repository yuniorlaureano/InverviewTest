using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTest.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<UserListDto?> Get(int id)
        {
            return await _userService.Get(id);
        }

        [HttpGet()]
        public async Task<IEnumerable<UserListDto>> Get([FromQuery]byte? age, [FromQuery]string? country)
        {
            return await _userService.Get(age, country);
        }

        [HttpPost()]
        public async Task<IActionResult> Post(UserCreationDto user)
        {
            await _userService.Add(user);
            return NoContent();
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> Post(IEnumerable<UserCreationDto> users)
        {
            await _userService.Add(users);
            return NoContent();
        }

        [HttpPut()]
        public async Task<IActionResult> Put(UserUpdateDto user)
        {
            await _userService.Update(user);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _userService.Delete(id);
            return NoContent();
        }
    }
}
