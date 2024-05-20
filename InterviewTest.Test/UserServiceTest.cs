using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using InterviewTest.Test.Util;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
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
            var data = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "MockData", "users.json"));
            var users = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<UserCreationDto>>(data, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new CustomDateTimeConverter("yyyy/MM/dd")
                }

            });

            await _userService.Add(users!);
            var insertedUsers = await _userService.Get(pageSize: 1000);

            Assert.True(insertedUsers.Any());
            Assert.Equal(1000, insertedUsers.Count());
        }

        [Fact]
        public async Task Should_Update_1_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUser = (await _userService.Get(country: user.Country)).First();

            insertedUser.LastName = "Modified " + Guid.NewGuid();

            var userUpdate = _mapper.Map<UserUpdateDto>(insertedUser);
            await _userService.Update(userUpdate);

            var updatedUser = await _userService.GetById(insertedUser.Id);

            Assert.Equal(updatedUser.LastName, insertedUser.LastName);
        }

        [Fact]
        public async Task Should_Create_1_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUser = (await _userService.Get(country: user.Country)).First();

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUser = (await _userService.Get(country: user.Country)).First();

            var userById = await _userService.GetById(insertedUser.Id);

            Assert.NotNull(userById);
        }

        [Fact]
        public async Task Should_Get_User_By_Email()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var userByEmaail = await _userService.GetByEmail(user.Email);

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

            var existingUser = await _userService.GetByEmail(user.Email);
            if (existingUser is null)
            {
                await _userService.Add(user);
            }
            var insertedUser = await _userService.GetByEmail(user.Email);

            Assert.NotNull(insertedUser);
        }
    }
}