using AutoFixture;
using AutoMapper;
using InterviewTest.Data;
using InterviewTest.Entity;
using InterviewTest.Test.Util;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
{
    public class UserTServiceTest
    {
        public IServiceProvider Services { get; private set; }
        public IFixture Fixture { get; private set; }
        public IUserTRepository _userTRepository { get; private set; }
        public IMapper Mapper { get; private set; }

        public UserTServiceTest()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Services = DependencyBuilder.GetServices();
            Fixture = new Fixture();
            _userTRepository = Services.GetRequiredService<IUserTRepository>();
            Mapper = Services.GetRequiredService<IMapper>();
        }


        [Fact]
        public async Task Should_Create_1000_Users()
        {
            var data = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "MockData", "users.json"));
            var users = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<User>>(data, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new CustomDateTimeConverter("yyyy/MM/dd")
                }

            });

            await _userTRepository.Add(users!);
            var insertedUsers = await _userTRepository.Get(pageSize: 1000);

            Assert.True(insertedUsers.Any());
            Assert.Equal(1000, insertedUsers.Count());
        }

        [Fact]
        public async Task Should_Update_1_User()
        {
            var user = Fixture
                .Build<User>()
                .Create();

            await _userTRepository.Add(user);

            var insertedUser = (await _userTRepository.Get(country: user.Country)).First();

            insertedUser.LastName = "Modifiend " + Guid.NewGuid();

            await _userTRepository.Update(user);

            var updatedUser = (await _userTRepository.Get(country: user.Country)).First();


            Assert.NotEqual(updatedUser.LastName, insertedUser.LastName);
        }

        [Fact]
        public async Task Should_Create_1_User()
        {
            var user = Fixture
                .Build<User>()
                .Create();

            await _userTRepository.Add(user);

            var insertedUser = (await _userTRepository.Get(country: user.Country)).First();

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            var user = Fixture
                .Build<User>()
                .Create();

            await _userTRepository.Add(user);

            var insertedUser = (await _userTRepository.Get(country: user.Country)).First();

            var userById = await _userTRepository.GetById(insertedUser.Id);

            Assert.NotNull(userById);
        }

        [Fact]
        public async Task Should_Get_User_By_Email()
        {
            var user = Fixture
                .Build<User>()
                .Create();

            await _userTRepository.Add(user);

            var userByEmaail = await _userTRepository.GetByEmail(user.Email);

            Assert.NotNull(userByEmaail);
        }
    }
}