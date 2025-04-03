using AutoMapper;
using Stockas.Application.Commands.TransactionCategory;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<CreateProductCommand, Product>();

        CreateMap<UpdateProductCommand, Product>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Transaction Mappings
        CreateMap<Transaction, TransactionDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Category.Type))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null));

        CreateMap<TransactionCategory, TransactionCategoryDTO>();
        CreateMap<CreateTransactionCategoryCommand, TransactionCategory>();

        CreateMap<UpdateTransactionCategoryCommand, TransactionCategory>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
