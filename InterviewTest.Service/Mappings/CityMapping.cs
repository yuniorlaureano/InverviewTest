using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class CityMapping : Profile
    {
        public CityMapping()
        {
            CreateMap<CityCreationDto, City>();
            CreateMap<CityUpdateDto, City>();
            CreateMap<City, CityListDto>();
            CreateMap<CityListDto, CityUpdateDto>();
            CreateMap<CityDetail, CityListDto>();
        }
    }
}
