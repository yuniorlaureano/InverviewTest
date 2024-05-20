using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserCreationDto, TemporalUser>();
            CreateMap<UserUpdateDto, TemporalUser>();
            CreateMap<TemporalUser, UserListDto>();
            CreateMap<UserListDto, UserUpdateDto>();
        }
    }
}
