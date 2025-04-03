using FluentValidation;
using Stockas.Application.Commands.TransactionCategory;

namespace Stockas.Application.Validators.TransactionCategory
{
    public class CreateTransactionCategoryValidator : AbstractValidator<CreateTransactionCategoryCommand>
    {
        public CreateTransactionCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .Must(t => t == "Income" || t == "Expense").WithMessage("Type must be either 'Income' or 'Expense'.");

            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
}
