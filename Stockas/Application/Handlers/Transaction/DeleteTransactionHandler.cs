using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.Transaction;
using Stockas.Entities;

namespace Stockas.Application.Handlers.Transaction
{
    public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionCommand, Unit>
    {
        private readonly StockasContext _context;
        private readonly ILogger<DeleteTransactionHandler> _logger;

        public DeleteTransactionHandler(StockasContext context, ILogger<DeleteTransactionHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.TransactionId == request.TransactionId &&
                    t.UserId == request.UserId,
                    cancellationToken);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found for this user.");
            }

            // Check if transaction is related to a product (buy/sell)
            if (transaction.ProductId != null)
            {
                throw new InvalidOperationException(
                    "Cannot delete product-related transactions directly. Adjust product stock first.");
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Transaction {TransactionId} deleted by user {UserId}",
                request.TransactionId, request.UserId);

            return Unit.Value;
        }
    }
}
