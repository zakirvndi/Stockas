using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands;
using Stockas.Entities;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers
{
    public class UpdateProductCategoryHandler : IRequestHandler<UpdateProductCategoryCommand, ProductCategoryDto>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public UpdateProductCategoryHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductCategoryDto> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.UserId == request.UserId, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Product Category with ID {request.CategoryId} not found or does not belong to this user.");
            }

            var existingCategory = await _context.ProductCategories
                .AnyAsync(c => c.CategoryName.ToLower() == request.CategoryName.ToLower()
                            && c.CategoryId != request.CategoryId
                            && c.UserId == request.UserId, cancellationToken); 

            if (existingCategory)
            {
                throw new ArgumentException("Category name must be unique for this user.");
            }

            category.CategoryName = request.CategoryName;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductCategoryDto>(category);
        }


    }
}
