using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.Transaction;
using Stockas.Entities;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers.Transaction
{
    public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand, TransactionDTO>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public UpdateTransactionHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDTO> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t =>
                    t.TransactionId == request.TransactionId &&
                    t.UserId == request.UserId,
                    cancellationToken);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found for this user.");
            }

            // Update category if name provided
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                var category = await _context.TransactionCategories
                    .FirstOrDefaultAsync(c =>
                        c.CategoryName == request.CategoryName &&
                        c.UserId == request.UserId,
                        cancellationToken);

                if (category == null)
                {
                    throw new KeyNotFoundException($"Category '{request.CategoryName}' not found for this user.");
                }
                transaction.Category = category;
            }

            // Update other allowed fields
            if (request.Amount.HasValue)
            {
                transaction.Amount = request.Amount.Value;
            }

            if (request.Description != null)
            {
                transaction.Description = request.Description;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TransactionDTO>(transaction);
        }
    }

}
