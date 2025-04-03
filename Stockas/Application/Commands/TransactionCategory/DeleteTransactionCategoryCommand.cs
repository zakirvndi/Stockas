using MediatR;

namespace Stockas.Application.Commands.TransactionCategory
{
    public class DeleteTransactionCategoryCommand : IRequest<Unit>
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
