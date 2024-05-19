using AutoMapper;
using InterviewTest.Common.Dto;
using InterviewTest.Entity;

namespace InterviewTest.Service.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<ProductCreationDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductListDto>();
            CreateMap<ProductListDto, ProductUpdateDto>();
        }
    }
}
