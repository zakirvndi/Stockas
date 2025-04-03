using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.Transaction;
using Stockas.Entities;
using TransactionEntity = Stockas.Entities.Transaction;
using Stockas.Models.DTOS;

namespace Stockas.Application.Handlers.Transaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionDTO>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;

        public CreateTransactionHandler(StockasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDTO> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            // Find category
            var category = await _context.TransactionCategories
                .FirstOrDefaultAsync(c =>
                    c.CategoryName == request.CategoryName &&
                    c.UserId == request.UserId,
                    cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category '{request.CategoryName}' not found. " +
                                          "Please create the category first.");
            }

            // Create transaction
            var transaction = new TransactionEntity
            {
                CategoryId = category.CategoryId,
                Amount = request.Amount,
                Description = request.Description,
                TransactionDate = DateTime.UtcNow,
                UserId = request.UserId,
                ProductId = null
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TransactionDTO>(transaction);
        }
    }

}
