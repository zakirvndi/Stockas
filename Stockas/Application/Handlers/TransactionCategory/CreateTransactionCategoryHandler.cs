using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.TransactionCategory;
using Stockas.Entities;
using TransactionCategoryEntity = Stockas.Entities.TransactionCategory;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers.TransactionCategory
{
    public class CreateTransactionCategoryHandler : IRequestHandler<CreateTransactionCategoryCommand, TransactionCategoryDTO>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public CreateTransactionCategoryHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionCategoryDTO> Handle(CreateTransactionCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if category with same name already exists for this user
            var existingCategory = await _context.TransactionCategories
                .AnyAsync(c =>
                    c.CategoryName.ToLower() == request.CategoryName.ToLower() &&
                    c.UserId == request.UserId,
                    cancellationToken);

            if (existingCategory)
            {
                throw new ArgumentException("Category name must be unique per user.");
            }

            // Validate type
            if (request.Type != "Income" && request.Type != "Expense")
            {
                throw new ArgumentException("Type must be either 'Income' or 'Expense'.");
            }

            var category = new TransactionCategoryEntity
            {
                CategoryName = request.CategoryName,
                Type = request.Type,
                UserId = request.UserId
            };

            _context.TransactionCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TransactionCategoryDTO>(category);
        }
    }

}
