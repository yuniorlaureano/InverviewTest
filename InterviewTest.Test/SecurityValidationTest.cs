using AutoFixture;
using FluentValidation;
using InterviewTest.Common.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
{
    public class SecurityValidationTest
    {
        private readonly IServiceProvider _services;
        private readonly IFixture _fixture;
        private readonly IValidator<UserLoginDto> _loginValidator;

        public SecurityValidationTest()
        {
            _services = DependencyBuilder.GetServices();
            _fixture = new Fixture();

            _loginValidator = _services.GetRequiredService<IValidator<UserLoginDto>>();
        }

        [Fact]
        public async Task Should_Return_Invalid_Email()
        {
            var credentials = _fixture
                .Build<UserLoginDto>()
                .Create();

            var result = await _loginValidator.ValidateAsync(credentials);

            Assert.False(result.IsValid);
            Assert.Equal(result.Errors[0].PropertyName, nameof(UserLoginDto.Email));
        }

        [Fact]
        public async Task Should_Return_Valid_Login()
        {
            var credentials = _fixture
                .Build<UserLoginDto>()
                .With(x => x.Email, "user@gmail.com")
                .Create();

            var result = await _loginValidator.ValidateAsync(credentials);

            Assert.True(result.IsValid);
        }
    }
}
