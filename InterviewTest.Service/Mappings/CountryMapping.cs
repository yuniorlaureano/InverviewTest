using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class CountryMapping : Profile
    {
        public CountryMapping()
        {
            CreateMap<CountryCreationDto, Country>();
            CreateMap<CountryUpdateDto, Country>();
            CreateMap<Country, CountryListDto>();
            CreateMap<CountryListDto, CountryUpdateDto>();
        }
    }
}
