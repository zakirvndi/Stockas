using MediatR;

namespace Stockas.Application.Commands.Transaction
{
    public class DeleteTransactionCommand : IRequest<Unit>
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
    }
}
