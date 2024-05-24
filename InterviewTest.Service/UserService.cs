using AutoMapper;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;

namespace InterviewTest.Service
{
    public interface IUserService
    {
        public Task<UserListDto?> GetByIdAsync(long id);
        Task<UserListDto?> GetByEmailAsync(string email);
        public Task<IEnumerable<UserListDto>> GetAsync(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
        public Task AddAsync(UserCreationDto userDto);
        public Task AddAsync(IEnumerable<UserCreationDto> userDtos);
        public Task UpdateAsync(UserUpdateDto userDto);
        public Task DeleteAsync(long id);
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

        public async Task AddAsync(UserCreationDto userDto)
        {
            var user = _mapper.Map<TemporalUser>(userDto);
            user.Password = PasswordHasher.HashPassword(userDto.Password);
            await _userRepository.AddAsync(user);
        }

        public async Task AddAsync(IEnumerable<UserCreationDto> userDtos)
        {
            var users = _mapper.Map<IEnumerable<TemporalUser>>(userDtos);
            await _userRepository.AddAsync(users);
        }

        public async Task DeleteAsync(long id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<UserListDto?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<UserListDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<IEnumerable<UserListDto>> GetAsync(int page = 1, int pageSize = 10, byte? age = null, string? country = null)
        {
            var users = await _userRepository.GetAsync(page, pageSize, age, country);
            return _mapper.Map<IEnumerable<UserListDto>>(users);
        }

        public async Task UpdateAsync(UserUpdateDto userDtos)
        {
            var user = _mapper.Map<TemporalUser>(userDtos);
            await _userRepository.UpdateAsync(user);
        }
    }
}
