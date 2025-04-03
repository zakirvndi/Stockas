using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.TransactionCategory;
using Stockas.Entities;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers.TransactionCategory
{
    public class UpdateTransactionCategoryHandler : IRequestHandler<UpdateTransactionCategoryCommand, TransactionCategoryDTO>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public UpdateTransactionCategoryHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionCategoryDTO> Handle(UpdateTransactionCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.TransactionCategories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == request.CategoryId &&
                    c.UserId == request.UserId,
                    cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found for this user.");
            }

            // Check if category name is being updated and if it's unique
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                var nameExists = await _context.TransactionCategories
                    .AnyAsync(c =>
                        c.CategoryName.ToLower() == request.CategoryName.ToLower() &&
                        c.CategoryId != request.CategoryId &&
                        c.UserId == request.UserId,
                        cancellationToken);

                if (nameExists)
                {
                    throw new ArgumentException("Category name must be unique per user.");
                }

                category.CategoryName = request.CategoryName;
            }

            // Check if type is being updated
            if (!string.IsNullOrEmpty(request.Type))
            {
                if (request.Type != "Income" && request.Type != "Expense")
                {
                    throw new ArgumentException("Type must be either 'Income' or 'Expense'.");
                }

                category.Type = request.Type;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TransactionCategoryDTO>(category);
        }
    }

}
