using AutoFixture;
using FluentValidation;
using InterviewTest.Common.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
{
    public class UserValidationTest
    {
        private readonly IServiceProvider _services;
        private readonly IFixture _fixture;
        private readonly IValidator<UserCreationDto> _userCreationValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;

        public UserValidationTest()
        {
            _services = DependencyBuilder.GetServices();
            _fixture = new Fixture();

            _userCreationValidator = _services.GetRequiredService<IValidator<UserCreationDto>>();
            _userUpdateValidator = _services.GetRequiredService<IValidator<UserUpdateDto>>();
        }

        [Fact]
        public async Task Should_Return_Invalid_User_Creation()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .With(x => x.FirstName, "")
                .With(x => x.LastName, "")
                .With(x => x.Age, 0)
                .Create();

            var result = await _userCreationValidator.ValidateAsync(user);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Should_Return_Invalid_User_Update()
        {
            var user = _fixture
                .Build<UserUpdateDto>()
                .With(x => x.FirstName, "")
                .With(x => x.LastName, "")
                .With(x => x.Age, 0)
                .Create();

            var result = await _userUpdateValidator.ValidateAsync(user);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Should_Return_Valid_User_Creation()
        {
            var user = _fixture
                .Build<UserCreationDto>()
                .With(x => x.Email, $"{Guid.NewGuid()}@.gmail.com")
                .Create();

            var result = await _userCreationValidator.ValidateAsync(user);

            Assert.True(result.IsValid);
        }
    }
}
