using FluentValidation;
using Stockas.Application.Commands.TransactionCategory;

namespace Stockas.Application.Validators.TransactionCategory
{
    public class UpdateTransactionCategoryValidator : AbstractValidator<UpdateTransactionCategoryCommand>
    {
        public UpdateTransactionCategoryValidator()
        {
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category ID must be greater than 0.");
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be greater than 0.");

            RuleFor(x => x.CategoryName)
                .NotEmpty().When(x => x.CategoryName != null)
                .MaximumLength(100).When(x => x.CategoryName != null);

            RuleFor(x => x.Type)
                .Must(t => t == "Income" || t == "Expense").When(x => x.Type != null);
        }
    }
}
