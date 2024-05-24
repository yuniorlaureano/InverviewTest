using AutoFixture;
using FluentAssertions;
using FluentValidation;
using InterviewTest.Api.Controllers;
using InterviewTest.Api.Util;
using InterviewTest.Common.Dto;
using InterviewTest.Data;
using InterviewTest.Service;
using InterviewTest.Service.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InterviewTest.Test.Tests.Controller
{
    public class UsersControllerTest
    {
        private readonly Mock<IUserService> _userService;
        private readonly IServiceProvider _services;
        private readonly UsersController _usersController;
        private readonly IFixture _fixture;

        public UsersControllerTest()
        {
            _services = DependencyBuilder.GetServices();

            _userService = new Mock<IUserService>();
            var userCreationValidator = _services.GetRequiredService<IValidator<UserCreationDto>>();
            var userUpdateValidator = _services.GetRequiredService<IValidator<UserUpdateDto>>();
            _fixture = new Fixture();
            _usersController = new UsersController(
            _userService.Object,
                userCreationValidator,
                userUpdateValidator);
        }

        [Fact]
        public async Task Should_Return_Ok_Get_User()
        {
            int userId = 1;
            var user = new UserListDto();

            _userService
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var result = await _usersController.Get(userId);
            result.Should().NotBeNull("A user was returned", result);
        }

        [Fact]
        public async Task Should_Return_Ok_Get_Users()
        {
            int page = 1;
            int pageSize = 10;
            byte? age = null;
            string? country = null;

            var users = new List<UserListDto>
            {
                new UserListDto()
            };

            _userService
                .Setup(x => x.GetAsync(page, pageSize, age, country))
                .ReturnsAsync(users);


            var result = await _usersController.Get();
            Assert.True(result.Any());
        }

        [Fact]
        public async Task Should_Return_NoContent_Post_User()
        {
            //Arrange
            var userCreation = _fixture
                .Build<UserCreationDto>()
                .With(x => x.Email, "admin@gmail.com")
                .Create();

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(userCreation));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(userCreation);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Should_Return_BadRequest_For_Email_Validation_Post_User()
        {
            //Arrange
            var userCreation = _fixture
                .Build<UserCreationDto>()
                .Create();

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(userCreation));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, configuration) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(userCreation);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            Assert.True(errors!.Errors.ContainsKey(nameof(userCreation.Email)));
        }

        [Fact]
        public async Task Should_Return_BadRequest_Validation_With_5_Errors_Post_User()
        {
            //Arrange
            var fieldsToValidate = new string[]
            {
                nameof(UserCreationDto.FirstName),
                nameof(UserCreationDto.LastName),
                nameof(UserCreationDto.Email),
                nameof(UserCreationDto.Password),
                nameof(UserCreationDto.Age),
            };

            var userCreation = new UserCreationDto();

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(userCreation));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(userCreation);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            Assert.True(fieldsToValidate.All(field => errors!.Errors.ContainsKey(field)));
            errors!.Errors.Count().Should().Be(5);
        }

        [Fact]
        public async Task Should_Return_NoContent_Post_User_List()
        {
            //Arrange
            var usersCreation = new List<UserCreationDto>();
            usersCreation.AddRange(
                    Enumerable.Range(1, 10).Select(x =>
                        _fixture
                            .Build<UserCreationDto>()
                            .With(x => x.Email, $"admin{x}@gmail.com")
                            .Create()
                    )
                );

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(It.IsAny<UserCreationDto>()));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(usersCreation);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Should_Return_BadRequest_For_Email_Validation_Post_User_List()
        {
            //Arrange
            var usersCreation = new List<UserCreationDto>();
            usersCreation.AddRange(
                    Enumerable.Range(1, 10).Select(x =>
                        _fixture
                            .Build<UserCreationDto>()
                            .Create()
                    )
                );

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(It.IsAny<UserCreationDto>()));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(usersCreation);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            Assert.True(errors!.Errors.ContainsKey(nameof(UserCreationDto.Email)));
        }

        [Fact]
        public async Task Should_Return_BadRequest_Validation_With_5_Errors_Post_User_List()
        {
            //Arrange
            var fieldsToValidate = new string[]
            {
                nameof(UserCreationDto.FirstName),
                nameof(UserCreationDto.LastName),
                nameof(UserCreationDto.Email),
                nameof(UserCreationDto.Password),
                nameof(UserCreationDto.Age),
            };

            var usersCreation = new List<UserCreationDto>();
            usersCreation.AddRange(
                    Enumerable.Range(1, 10).Select(x =>
                        new UserCreationDto()
                    )
                );

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.AddAsync(It.IsAny<UserCreationDto>()));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Post(usersCreation);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            Assert.True(fieldsToValidate.All(field => errors!.Errors.ContainsKey(field)));
            errors!.Errors.Count().Should().Be(5);
        }

        [Fact]
        public async Task Should_Return_NoContent_Put_User()
        {
            //Arrange
            var userUpdate = _fixture
                .Build<UserUpdateDto>()
                .With(x => x.Id, 1)
                .With(x => x.Email, "admin@gmail.com")
                .Create();

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.UpdateAsync(userUpdate));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            userService
                .Setup(x => x.GetByIdAsync(userUpdate.Id))
                .ReturnsAsync(() => userList);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Put(userUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Should_Return_BadRequest_NonExistent_User_Put_User()
        {
            //Arrange
            var userUpdate = _fixture
                .Build<UserUpdateDto>()
                .With(x => x.Id, 1)
                .With(x => x.Email, "admin@gmail.com")
                .Create();

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.UpdateAsync(userUpdate));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            userService
                .Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Put(userUpdate);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            errors.Errors.Should().ContainKey("Id");
        }

        [Fact]
        public async Task Should_Return_BadRequest_Validation_With_5_Errors_Put_User()
        {
            //Arrange
            var fieldsToValidate = new string[]
            {
                nameof(UserUpdateDto.FirstName),
                nameof(UserUpdateDto.LastName),
                nameof(UserUpdateDto.Email),
                nameof(UserUpdateDto.Age),
            };

            var userUpdate = new UserUpdateDto();

            var userList = new UserListDto()
            {
                Id = 1
            };

            var userService = new Mock<IUserService>();
            var userRepository = new Mock<IUserRepository>();

            userService
               .Setup(x => x.UpdateAsync(userUpdate));

            userService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            userService
                .Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            var services = DependencyBuilder.GetServices((serviceCollection, _) =>
            {
                serviceCollection.AddScoped<IUserRepository>((_) => userRepository.Object);
                serviceCollection.AddScoped<IUserService>((_) => userService.Object);
                serviceCollection.AddMappings();
                serviceCollection.AddValidators();
            });


            var usersController = new UsersController(
                userService.Object,
                services.GetRequiredService<IValidator<UserCreationDto>>(),
                services.GetRequiredService<IValidator<UserUpdateDto>>()
            );

            //Act
            var result = await usersController.Put(userUpdate);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var errors = (result as BadRequestObjectResult)!.Value as ErrorResult;
            Assert.True(fieldsToValidate.All(field => errors!.Errors.ContainsKey(field)));
            errors!.Errors.Count().Should().Be(5);
        }

        [Fact]
        public async Task Should_Return_NoContent_Delete_User()
        {
            int userId = 1;
            var user = new UserListDto();

            _userService
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _userService
                .Setup(x => x.DeleteAsync(userId));

            var result = await _usersController.Delete(userId);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Should_Return_BadRequest_Delete_User()
        {
            int userId = 1;
            var user = new UserListDto();

            _userService
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(() => null);

            _userService
                .Setup(x => x.DeleteAsync(userId));

            var result = await _usersController.Delete(userId);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
