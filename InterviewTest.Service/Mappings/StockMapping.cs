using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class StockMapping : Profile
    {
        public StockMapping()
        {
            CreateMap<StockCreationDto, Stock>();
            CreateMap<StockUpdateDto, Stock>();
            CreateMap<Stock, StockListDto>();
            CreateMap<StockListDto, StockUpdateDto>();
        }
    }
}
