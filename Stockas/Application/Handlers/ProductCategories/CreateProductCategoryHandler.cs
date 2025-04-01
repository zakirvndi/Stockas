using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands;
using Stockas.Entities;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers
{
    public class CreateProductCategoryHandler : IRequestHandler<CreateProductCategoryCommand, ProductCategoryDto>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public CreateProductCategoryHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductCategoryDto> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            // Cek apakah kategori dengan nama yang sama sudah ada
            var existingCategory = await _context.ProductCategories
                .AnyAsync(c => c.CategoryName.ToLower() == request.CategoryName.ToLower(), cancellationToken);

            if (existingCategory)
            {
                throw new ArgumentException("Category name must be unique.");
            }

            
            var newCategory = new ProductCategory
            {
                CategoryName = request.CategoryName
            };

            _context.ProductCategories.Add(newCategory);
            await _context.SaveChangesAsync(cancellationToken);

           
            return _mapper.Map<ProductCategoryDto>(newCategory);
        }
    }
}
