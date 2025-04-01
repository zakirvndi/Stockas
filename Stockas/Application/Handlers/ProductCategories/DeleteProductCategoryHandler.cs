using MediatR;
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
        var category = await _context.ProductCategories.FindAsync(request.CategoryId);

        if (category == null)
        {
            throw new KeyNotFoundException($"Product Category with ID {request.CategoryId} not found.");
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value; 
    }
}
