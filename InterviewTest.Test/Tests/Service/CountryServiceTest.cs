using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Common.Extensions;
using InterviewTest.Data;
using InterviewTest.Data.Extensions;
using InterviewTest.Service.Extensions;
using InterviewTest.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewTest.Test.Tests.Service
{
    public class CountryServiceTest
    {
        private readonly IServiceProvider _services;
        private readonly IFixture _fixture;
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountryServiceTest()
        {
            _services = DependencyBuilder.GetServices((collection, _) =>
            {
                collection.AddRepositories();
                collection.AddServices();
                collection.AddMappings();
                collection.AddValidators();
                collection.AddCommons();

                var mockLogger = new Mock<ILogger<CountryRepository>>();
                collection.AddSingleton<ILogger<CountryRepository>>(mockLogger.Object);
            });
            _fixture = new Fixture();
            _countryService = _services.GetRequiredService<ICountryService>();
            _mapper = _services.GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task Should_Create_country()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.AddAsync(country);

            var result = await _countryService.GetAsync(name: country.Name);

            Assert.True(result.Data.Any());
            Assert.True(result.IsSuccess);
            Assert.False(result.Errors.Any());
        }

        [Fact]
        public async Task Should_Update_Country()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.AddAsync(country);

            var insertedcountry = (await _countryService.GetAsync(name: country.Name)).Data.First();

            insertedcountry.Name = "Modified " + Guid.NewGuid();

            var countryUpdate = _mapper.Map<CountryUpdateDto>(insertedcountry);
            await _countryService.UpdateAsync(countryUpdate);

            var updatedcountry = await _countryService.GetByIdAsync(insertedcountry.Id);

            Assert.Equal(updatedcountry.Data.Name, insertedcountry.Name);
        }

        [Fact]
        public async Task Should_Create_Country()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.AddAsync(country);

            var insertedcountry = (await _countryService.GetAsync(name: country.Name)).Data.First();

            Assert.NotNull(insertedcountry);
        }

        [Fact]
        public async Task Should_Get_Country_By_Id()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.AddAsync(country);

            var insertedcountry = (await _countryService.GetAsync(name: country.Name)).Data.First();

            var countryById = await _countryService.GetByIdAsync(insertedcountry.Id);

            Assert.NotNull(countryById.Data);
        }
    }
}