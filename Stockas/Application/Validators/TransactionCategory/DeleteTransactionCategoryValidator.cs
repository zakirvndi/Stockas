using FluentValidation;
using Stockas.Application.Commands.TransactionCategory;

namespace Stockas.Application.Validators.TransactionCategory
{
    public class DeleteTransactionCategoryValidator : AbstractValidator<DeleteTransactionCategoryCommand>
    {
        public DeleteTransactionCategoryValidator()
        {
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category ID must be greater than 0.");
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
}
