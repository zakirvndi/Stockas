using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.TransactionCategory;
using Stockas.Entities;

namespace Stockas.Application.Handlers.TransactionCategory
{
    public class DeleteTransactionCategoryHandler : IRequestHandler<DeleteTransactionCategoryCommand, Unit>
    {
        private readonly StockasContext _context;

        public DeleteTransactionCategoryHandler(StockasContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTransactionCategoryCommand request, CancellationToken cancellationToken)
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

            // Check if category is used in any transactions
            var isUsed = await _context.Transactions
                .AnyAsync(t => t.CategoryId == request.CategoryId, cancellationToken);

            if (isUsed)
            {
                throw new InvalidOperationException("Cannot delete category that is used in transactions.");
            }

            _context.TransactionCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
