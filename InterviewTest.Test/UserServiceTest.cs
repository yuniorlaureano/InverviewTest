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
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            _services = DependencyBuilder.GetServices();
            _fixture = new Fixture();
            _userService = _services.GetRequiredService<IUserService>();
            _mapper = _services.GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedId = (await _userService.Get()).FirstOrDefault()?.Id;
            var insertedUser = await _userService.Get(insertedId ?? 0);

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Should_Get_Users()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUsers = await _userService.Get();

            Assert.NotNull(insertedUsers);
        }

        [Fact]
        public async Task Should_Get_User_By_Country()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUsers = await _userService.Get(country: user.Country);

            Assert.NotNull(insertedUsers);
        }

        [Fact]
        public async Task Should_Get_User_By_Age()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUsers = await _userService.Get(age: user.Age);

            Assert.NotNull(insertedUsers);
        }

        [Fact]
        public async Task Should_Get_User_By_Age_And_Country()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUsers = await _userService.Get(age: user.Age, country: user.Country);

            Assert.NotNull(insertedUsers);
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);
            var insertedUser = await _userService.Get(country: user.Country);

            Assert.NotNull(insertedUser);
            Assert.Equal(insertedUser.First().Country, user.Country);
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
            var insertedUsers = await _userService.Get();

            Assert.NotNull(insertedUsers);
            Assert.True(insertedUsers.Count() >= 1000);
        }

        [Fact]
        public async Task Should_Update_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);
            
            var insertedUsers = await _userService.Get(country: user.Country);
            var insertedUser = insertedUsers.First();

            var userToUpdate = _mapper.Map<UserUpdateDto>(insertedUser);
            
            userToUpdate.Country = Guid.NewGuid().ToString(); 
            await _userService.Update(userToUpdate);

            var updatedUser = await _userService.Get(userToUpdate.Id);

            Assert.NotNull(updatedUser);
            Assert.Equal(updatedUser.Country, userToUpdate.Country);
        }

        [Fact]
        public async Task Should_Delete_User()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .Create();

            await _userService.Add(user);

            var insertedUsers = await _userService.Get(country: user.Country);
            var insertedUser = insertedUsers.First();

            var beforeRemove = await _userService.Get(insertedUser.Id);
            await _userService.Delete(insertedUser.Id);
            var afterRemove = await _userService.Get(insertedUser.Id);

            Assert.NotNull(beforeRemove);
            Assert.Null(afterRemove);
        }
    }
}