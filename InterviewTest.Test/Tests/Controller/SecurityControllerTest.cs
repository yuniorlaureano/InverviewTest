using AutoFixture;
using FluentAssertions;
using FluentValidation;
using InterviewTest.Api.Controllers;
using InterviewTest.Api.Util;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Service;
using InterviewTest.Service.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InterviewTest.Test.Tests.Controller
{
    public class SecurityControllerTest
    {
        private readonly IFixture _fixture;

        public SecurityControllerTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Should_Return_Ok_With_Valid_JwtToken_Login()
        {
            //Arrange
            var user = new UserLoginDto()
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            var userList = _fixture
                .Build<UserListDto>()
                .With(x => x.Email, "admin@gmail.com")
                .With(x => x.Password, PasswordHasher.HashPassword("admin"))
                .Create();

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
                .Setup(x => x.GetByEmail(It.IsAny<string>()))
                .ReturnsAsync(() => userList);

            var services = DependencyBuilder.GetServices((serviceCollection, configuration) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddJwt(configuration);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });

            var jwtService = services.GetRequiredService<IJwtService>();
            var userLoginValidator = services.GetRequiredService<IValidator<UserLoginDto>>();

            var securityController = new SecurityController(
                 jwtService,
                 userService.Object,
                 userLoginValidator);

            //Act
            var result = await securityController.Login(user);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            var token = (result as OkObjectResult)!.Value!.ToString();
            var isValid = await jwtService.ValidateToken(token!);
            token.Should().NotBeNull();
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Return_BadRequest_Validation_No_User_Provided()
        {
            //Arrange
            var user = new UserLoginDto();

            var userList = _fixture
                .Build<UserListDto>()
                .With(x => x.Email, "admin@gmail.com")
                .With(x => x.Password, PasswordHasher.HashPassword("admin"))
                .Create();

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
                .Setup(x => x.GetByEmail(It.IsAny<string>()))
                .ReturnsAsync(() => userList);

            var services = DependencyBuilder.GetServices((serviceCollection, configuration) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddJwt(configuration);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });

            var jwtService = services.GetRequiredService<IJwtService>();
            var userLoginValidator = services.GetRequiredService<IValidator<UserLoginDto>>();

            var securityController = new SecurityController(
                 jwtService,
                 userService.Object,
                 userLoginValidator);

            //Act
            var result = await securityController.Login(user);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var error = (result as BadRequestObjectResult)!.Value as ErrorResult;
            error!.Errors.Should().ContainKeys("Email", "Password");
        }

        [Fact]
        public async Task Should_Return_NotFound()
        {
            //Arrange
            var user = new UserLoginDto()
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
                .Setup(x => x.GetByEmail(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, configuration) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddJwt(configuration);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });

            var jwtService = services.GetRequiredService<IJwtService>();
            var userLoginValidator = services.GetRequiredService<IValidator<UserLoginDto>>();

            var securityController = new SecurityController(
                 jwtService,
                 userService.Object,
                 userLoginValidator);

            //Act
            var result = await securityController.Login(user);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Should_Return_Unauthorized_Invalid_Password()
        {
            //Arrange
            var user = new UserLoginDto()
            {
                Email = "admin@gmail.com",
                Password = "invalid_password"
            };

            var userList = _fixture
                .Build<UserListDto>()
                .With(x => x.Email, "admin@gmail.com")
                .With(x => x.Password, PasswordHasher.HashPassword("admin"))
                .Create();

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
                .Setup(x => x.GetByEmail(It.IsAny<string>()))
                .ReturnsAsync(() => userList);

            var services = DependencyBuilder.GetServices((serviceCollection, configuration) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddJwt(configuration);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });

            var jwtService = services.GetRequiredService<IJwtService>();
            var userLoginValidator = services.GetRequiredService<IValidator<UserLoginDto>>();

            var securityController = new SecurityController(
                 jwtService,
                 userService.Object,
                 userLoginValidator);

            //Act
            var result = await securityController.Login(user);

            //Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

    }
}
