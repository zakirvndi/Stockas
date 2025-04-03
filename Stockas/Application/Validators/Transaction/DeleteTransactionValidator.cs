using FluentValidation;
using Stockas.Application.Commands.Transaction;

namespace Stockas.Application.Validators.Transaction
{
    public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionCommand>
    {
        public DeleteTransactionValidator()
        {
            RuleFor(x => x.TransactionId).GreaterThan(0).WithMessage("Transaction ID must be greater than 0.");
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
}
