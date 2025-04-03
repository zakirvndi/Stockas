using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands;
using Stockas.Entities;

public class DeleteProductCategoryHandler : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    private readonly StockasContext _context;

    public DeleteProductCategoryHandler(StockasContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.UserId == request.UserId, cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException($"Product Category with ID {request.CategoryId} not found or does not belong to this user.");
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

}
