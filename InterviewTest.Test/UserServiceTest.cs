using AutoFixture;
using InterviewTest.Common.Dto;
using InterviewTest.Test.Fixtures;
using InterviewTest.Test.Util;

namespace InterviewTest.Test
{
    [Collection(nameof(UserServiceTestCollection))]
    public class UserServiceTest
    {
        private readonly UserServiceTestFixture _userServiceTestFixture;

        public UserServiceTest(UserServiceTestFixture userServiceTestFixture)
        {
            _userServiceTestFixture = userServiceTestFixture;
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            await _userServiceTestFixture.MockUser();

            var insertedId = (await _userServiceTestFixture.UserService.Get()).FirstOrDefault()?.Id;
            var insertedUser = await _userServiceTestFixture.UserService.GetById(insertedId ?? 0);

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Should_Get_Users()
        {
            await _userServiceTestFixture.MockUser();
            var insertedUsers = await _userServiceTestFixture.UserService.Get();

            Assert.True(insertedUsers.Any());
        }

        [Fact]
        public async Task Should_Get_User_By_Country()
        {
            var user = await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(country: user.Country);

            Assert.True(insertedUsers.Any());
        }

        [Fact]
        public async Task Should_Get_User_By_Age()
        {
            var user = await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(age: user.Age);

            Assert.True(insertedUsers.Any());
        }

        [Fact]
        public async Task Should_Get_User_By_Email()
        {
            var user = await _userServiceTestFixture.MockUser();

            var existingUser = await _userServiceTestFixture.UserService.GetByEmail(user.Email);

            Assert.NotNull(existingUser);
        }

        [Fact]
        public async Task Should_Get_User_By_Age_And_Country()
        {
            var user = await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(age: user.Age, country: user.Country);

            Assert.True(insertedUsers.Any());
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var user = _userServiceTestFixture.Fixture
                .Build<UserCreationDto>()
                .With(x => x.Email, "admin@gmail.com")
                .With(x => x.Password, "admin")
                .Create();

            await _userServiceTestFixture.UserService.Add(user);
            var insertedUser = await _userServiceTestFixture.UserService.Get(country: user.Country);

            Assert.True(insertedUser.Any());
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

            await _userServiceTestFixture.UserService.Add(users!);
            var insertedUsers = await _userServiceTestFixture.UserService.Get(pageSize: 1000);

            Assert.True(insertedUsers.Any());
            Assert.Equal(1000, insertedUsers.Count());
        }

        [Fact]
        public async Task Should_Update_User()
        {
            var user = await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(country: user.Country);
            var insertedUser = insertedUsers.First();

            var userToUpdate = _userServiceTestFixture.Mapper.Map<UserUpdateDto>(insertedUser);

            userToUpdate.Country = Guid.NewGuid().ToString();
            await _userServiceTestFixture.UserService.Update(userToUpdate);

            var updatedUser = await _userServiceTestFixture.UserService.GetById(userToUpdate.Id);

            Assert.NotNull(updatedUser);
            Assert.Equal(updatedUser.Country, userToUpdate.Country);
        }

        [Fact]
        public async Task Should_Delete_User()
        {
            var user = await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(country: user.Country);
            var insertedUser = insertedUsers.First();

            var beforeRemove = await _userServiceTestFixture.UserService.GetById(insertedUser.Id);
            await _userServiceTestFixture.UserService.Delete(insertedUser.Id);
            var afterRemove = await _userServiceTestFixture.UserService.GetById(insertedUser.Id);

            Assert.NotNull(beforeRemove);
            Assert.Null(afterRemove);
        }

        [Fact]
        public async Task Should_Paginate_To_5_Users()
        {
            await _userServiceTestFixture.MockUser();

            var insertedUsers = await _userServiceTestFixture.UserService.Get(pageSize: 5);

            Assert.True(insertedUsers.Any());
            Assert.Equal(5, insertedUsers.Count());
        }
    }
}