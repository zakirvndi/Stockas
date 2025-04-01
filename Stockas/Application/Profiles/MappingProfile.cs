using AutoMapper;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<CreateProductCommand, Product>();

        CreateMap<UpdateProductCommand, Product>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
