using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Service.Interfaces;
using InterviewTest.Test.Util;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test.Tests.Service
{
    public class UserServiceTest
    {
        private readonly IServiceProvider _services;
        private readonly IFixture _fixture;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserServiceTest()
        {
            _services = DependencyBuilder.GetServices();
            _fixture = new Fixture();
            _userService = _services.GetRequiredService<IUserService>();
            _mapper = _services.GetRequiredService<IMapper>();
        }


        [Fact]
        public async Task Should_Create_1000_Users()
        {
            var data = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockData", "users.json"));
            var users = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<UserCreationDto>>(data, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new CustomDateTimeConverter("yyyy/MM/dd")
                }

            });

            await _userService.AddAsync(users!);
            var insertedUsers = await _userService.GetAsync(pageSize: 1000);

            Assert.True(insertedUsers.Any());
            Assert.Equal(1000, insertedUsers.Count());
        }

        [Fact]
        public async Task Should_Update_1_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.AddAsync(user);

            var insertedUser = (await _userService.GetAsync(country: user.Country)).First();

            insertedUser.LastName = "Modified " + Guid.NewGuid();

            var userUpdate = _mapper.Map<UserUpdateDto>(insertedUser);
            await _userService.UpdateAsync(userUpdate);

            var updatedUser = await _userService.GetByIdAsync(insertedUser.Id);

            Assert.Equal(updatedUser.LastName, insertedUser.LastName);
        }

        [Fact]
        public async Task Should_Create_1_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.AddAsync(user);

            var insertedUser = (await _userService.GetAsync(country: user.Country)).First();

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.AddAsync(user);

            var insertedUser = (await _userService.GetAsync(country: user.Country)).First();

            var userById = await _userService.GetByIdAsync(insertedUser.Id);

            Assert.NotNull(userById);
        }

        [Fact]
        public async Task Should_Get_User_By_Email()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.AddAsync(user);

            var userByEmaail = await _userService.GetByEmailAsync(user.Email);

            Assert.NotNull(userByEmaail);
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .With(x => x.Email, "admin@gmail.com")
            .With(x => x.Password, "admin")
                .Create();

            var existingUser = await _userService.GetByEmailAsync(user.Email);
            if (existingUser is null)
            {
                await _userService.AddAsync(user);
            }
            var insertedUser = await _userService.GetByEmailAsync(user.Email);

            Assert.NotNull(insertedUser);
        }
    }
}