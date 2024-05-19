using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class StockDetailMapping : Profile
    {
        public StockDetailMapping()
        {
            CreateMap<StockDetail, StockDetailListDto>();
            CreateMap<StockDetailCreationDto, StockDetail>();
            CreateMap<StockDetailListDto, StockDetailCreationDto>();
        }
    }
}
