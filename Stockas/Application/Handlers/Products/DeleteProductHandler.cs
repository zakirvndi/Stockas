using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stockas.Entities;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly StockasContext _context;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(StockasContext context, ILogger<DeleteProductHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
{
    var product = await _context.Products.FindAsync(request.ProductId);
    if (product == null)
        throw new KeyNotFoundException("Product not found.");

    if (product.UserId != request.UserId)
        throw new UnauthorizedAccessException("You are not authorized to delete this product.");

    bool isUsedInTransactions = await _context.Transactions.AnyAsync(t => t.ProductId == request.ProductId);
    if (isUsedInTransactions)
        throw new InvalidOperationException("Product is referenced in transactions and cannot be deleted.");

    _context.Products.Remove(product);
    await _context.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Product {ProductId} deleted by user {UserId}", request.ProductId, request.UserId);
}

}
