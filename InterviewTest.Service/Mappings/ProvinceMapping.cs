using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class ProvinceMapping : Profile
    {
        public ProvinceMapping()
        {
            CreateMap<ProvinceCreationDto, Province>();
            CreateMap<ProvinceUpdateDto, Province>();
            CreateMap<Province, ProvinceListDto>();
            CreateMap<ProvinceListDto, ProvinceUpdateDto>();
            CreateMap<ProvinceDetail, ProvinceListDto>();
        }
    }
}
