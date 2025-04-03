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
            
            var existingCategory = await _context.ProductCategories
                .AnyAsync(c => c.CategoryName.ToLower() == request.CategoryName.ToLower() && c.UserId == request.UserId, cancellationToken);

            if (existingCategory)
            {
                throw new ArgumentException("Category name must be unique for each user.");
            }

            var newCategory = new ProductCategory
            {
                CategoryName = request.CategoryName,
                UserId = request.UserId 
            };

            _context.ProductCategories.Add(newCategory);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductCategoryDto>(newCategory);
        }

    }
}
