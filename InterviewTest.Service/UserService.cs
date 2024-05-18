using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IUserService
    {
        public Task<UserListDto?> Get(long id);
        public Task<IEnumerable<UserListDto>> Get(byte? age = null, string? country = null);
        public Task Add(UserCreationDto userDto);
        public Task Add(IEnumerable<UserCreationDto> userDtos);
        public Task Update(UserUpdateDto userDto);
        public Task Delete(long id);
    }

    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task Add(UserCreationDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.Add(user);
        }

        public async Task Add(IEnumerable<UserCreationDto> userDtos)
        {
            var users = _mapper.Map<IEnumerable<User>>(userDtos);
            await _userRepository.Add(users);
        }

        public async Task Delete(long id)
        {
            await _userRepository.Delete(id);
        }

        public async Task<UserListDto?> Get(long id)
        {
            var user = await _userRepository.Get(id);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<IEnumerable<UserListDto>> Get(byte? age = null, string? country = null)
        {
            var users = await _userRepository.Get(age, country);
            return _mapper.Map<IEnumerable<UserListDto>>(users);
        }

        public async Task Update(UserUpdateDto userDtos)
        {
            var user = _mapper.Map<User>(userDtos);
            await _userRepository.Update(user);
        }
    }
}
