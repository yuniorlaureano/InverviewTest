using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserCreationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserListDto>();
            CreateMap<UserListDto, UserUpdateDto>();
        }
    }
}
