using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDTO>
{
    private readonly StockasContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(StockasContext context, IMapper mapper, ILogger<UpdateProductHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
        .Include(p => p.Category) 
        .FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        if (product.UserId != request.UserId)
            throw new UnauthorizedAccessException("You are not authorized to update this product.");

        if (!string.IsNullOrEmpty(request.ProductName)) product.ProductName = request.ProductName;
        if (request.Quantity.HasValue) product.Quantity = request.Quantity.Value;
        if (request.PurchasePrice.HasValue) product.PurchasePrice = request.PurchasePrice.Value;
        if (request.SellingPrice.HasValue) product.SellingPrice = request.SellingPrice.Value;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product updated: {ProductId} by user {UserId}", product.ProductId, request.UserId);

        return _mapper.Map<ProductDTO>(product);
    }

}
