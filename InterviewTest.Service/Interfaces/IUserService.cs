using InterviewTest.Common.Dto;

namespace InterviewTest.Service.Interfaces
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
}
