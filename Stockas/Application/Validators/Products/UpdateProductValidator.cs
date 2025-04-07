using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly StockasContext _context;

    public UpdateProductValidator(StockasContext context)
    {
        _context = context;

        RuleFor(x => x.ProductId)
            .MustAsync(async (id, cancellation) =>
                await _context.Products.AnyAsync(p => p.ProductId == id, cancellation))
            .WithMessage("Product not found.");
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .When(x => x.ProductName != null)
            .WithMessage("Product Name cannot be empty.")
            .MustAsync(async (command, name, cancellation) =>
                !await _context.Products.AnyAsync(p =>
                    p.ProductName == name &&
                    p.ProductId != command.ProductId,
                    cancellation))
            .When(x => x.ProductName != null)
            .WithMessage("Product Name must be unique.");

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, cancellation) =>
                await _context.ProductCategories.AnyAsync(c => c.CategoryId == categoryId, cancellation))
            .When(x => x.CategoryId != null)
            .WithMessage("Category does not exist.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Quantity != null)
            .WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .When(x => x.PurchasePrice != null)
            .WithMessage("Purchase Price must be greater than 0.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0)
            .When(x => x.SellingPrice != null)
            .WithMessage("Selling Price must be greater than 0.");

        // Minimal satu field harus diupdate
        RuleFor(x => x)
            .Must(x => x.ProductName != null || x.CategoryId != null || x.Quantity != null || x.PurchasePrice != null || x.SellingPrice != null)
            .WithMessage("At least one field must be updated.");
    }
}
