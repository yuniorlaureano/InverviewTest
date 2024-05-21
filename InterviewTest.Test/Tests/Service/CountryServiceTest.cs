using AutoFixture;
using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Service;
using Microsoft.Extensions.DependencyInjection;

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
            _services = DependencyBuilder.GetServices();
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

            var countries = await _countryService.Get(name: country.Name);
            Assert.NotNull(countries);
        }

        [Fact]
        public async Task Should_Update_Country()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.Add(country);

            var insertedcountry = (await _countryService.Get(name: country.Name)).First();

            insertedcountry.Name = "Modified " + Guid.NewGuid();

            var countryUpdate = _mapper.Map<CountryUpdateDto>(insertedcountry);
            await _countryService.Update(countryUpdate);

            var updatedcountry = await _countryService.GetById(insertedcountry.Id);

            Assert.Equal(updatedcountry.Name, insertedcountry.Name);
        }

        [Fact]
        public async Task Should_Create_Country()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.Add(country);

            var insertedcountry = (await _countryService.Get(name: country.Name)).First();

            Assert.NotNull(insertedcountry);
        }

        [Fact]
        public async Task Should_Get_Country_By_Id()
        {
            var country = _fixture
                .Build<CountryCreationDto>()
                .Create();

            await _countryService.Add(country);

            var insertedcountry = (await _countryService.Get(name: country.Name)).First();

            var countryById = await _countryService.GetById(insertedcountry.Id);

            Assert.NotNull(countryById);
        }
    }
}