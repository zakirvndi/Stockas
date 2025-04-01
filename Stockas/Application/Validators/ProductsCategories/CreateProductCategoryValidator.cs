using FluentValidation;
using Stockas.Application.Commands;

namespace Stockas.Application.Validators
{
    public class CreateProductCategoryValidator : AbstractValidator<CreateProductCategoryCommand>
    {
        public CreateProductCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
        }
    }
}
