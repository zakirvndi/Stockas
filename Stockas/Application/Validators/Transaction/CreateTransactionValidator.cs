using FluentValidation;
using Stockas.Application.Commands.Transaction;

namespace Stockas.Application.Validators.Transaction
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionValidator()
        {
            RuleFor(x => x.CategoryName)
           .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("Amount cannot be zero");

            RuleFor(x => x.Description)
                .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
