using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Entity;
using InterviewTest.Service;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test.Fixtures
{
    public class UserServiceTestFixture : IAsyncLifetime
    {
        public IServiceProvider Services { get; private set; }
        public IFixture Fixture { get; private set; }
        public IUserService UserService { get; private set; }
        public IMapper Mapper { get; private set; }
        public IADOCommand AdoCommand { get; private set; }

        public UserServiceTestFixture()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Services = DependencyBuilder.GetServices();
            Fixture = new Fixture();
            UserService = Services.GetRequiredService<IUserService>();
            Mapper = Services.GetRequiredService<IMapper>();
            AdoCommand = Services.GetRequiredService<IADOCommand>();
        }

        public async Task<UserCreationDto> MockUser()
        {
            var user = Fixture
                .Build<UserCreationDto>()
                .Create();

            await UserService.Add(user);
            return user;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }

    [CollectionDefinition(nameof(UserServiceTestCollection))]
    public class UserServiceTestCollection : ICollectionFixture<UserServiceTestFixture>
    {
    }
}
