using FluentValidation;
using Stockas.Application.Commands.Transaction;

namespace Stockas.Application.Validators.Transaction
{
    public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionCommand>
    {
        public UpdateTransactionValidator()
        {
            RuleFor(x => x.CategoryName)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.CategoryName));

            RuleFor(x => x.Amount)
                .NotEqual(0).When(x => x.Amount.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(255).When(x => x.Description != null);

            RuleFor(x => x)
                .Must(x => x.CategoryName != null || x.Amount.HasValue || x.Description != null)
                .WithMessage("At least one field must be provided");
        }
    }
}
