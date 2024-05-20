using AutoMapper;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IUserService
    {
        public Task<UserListDto?> GetById(long id);
        Task<UserListDto?> GetByEmail(string email);
        public Task<IEnumerable<UserListDto>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
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
            var user = _mapper.Map<TemporalUser>(userDto);
            user.Password = PasswordHasher.HashPassword(userDto.Password);
            await _userRepository.Add(user);
        }

        public async Task Add(IEnumerable<UserCreationDto> userDtos)
        {
            var users = _mapper.Map<IEnumerable<TemporalUser>>(userDtos);
            await _userRepository.Add(users);
        }

        public async Task Delete(long id)
        {
            await _userRepository.Delete(id);
        }

        public async Task<UserListDto?> GetById(long id)
        {
            var user = await _userRepository.GetById(id);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<UserListDto?> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<IEnumerable<UserListDto>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null)
        {
            var users = await _userRepository.Get(page, pageSize, age, country);
            return _mapper.Map<IEnumerable<UserListDto>>(users);
        }

        public async Task Update(UserUpdateDto userDtos)
        {
            var user = _mapper.Map<TemporalUser>(userDtos);
            await _userRepository.Update(user);
        }
    }
}
