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
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        // Simpan nilai lama untuk logging
        var oldProduct = new
        {
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Quantity = product.Quantity,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice
        };

        // Validasi CategoryId jika diubah
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.ProductCategories
                .AnyAsync(c => c.CategoryId == request.CategoryId.Value);

            if (!categoryExists)
                throw new ArgumentException("Category does not exist.");

            product.CategoryId = request.CategoryId.Value;
        }

        // Update hanya field yang dikirim
        if (!string.IsNullOrEmpty(request.ProductName)) product.ProductName = request.ProductName;
        if (request.Quantity.HasValue) product.Quantity = request.Quantity.Value;
        if (request.PurchasePrice.HasValue) product.PurchasePrice = request.PurchasePrice.Value;
        if (request.SellingPrice.HasValue) product.SellingPrice = request.SellingPrice.Value;

        await _context.SaveChangesAsync(cancellationToken);

        // Logging perubahan
        _logger.LogInformation("Product updated: {ProductId} | Name: {OldName} -> {NewName} | CategoryId: {OldCategory} -> {NewCategory} | Quantity: {OldQty} -> {NewQty} | Price: {OldPrice} -> {NewPrice}",
            product.ProductId, oldProduct.ProductName, product.ProductName, oldProduct.CategoryId, product.CategoryId,
            oldProduct.Quantity, product.Quantity, oldProduct.PurchasePrice, product.PurchasePrice);

        // Mapping ke DTO dan return
        var updatedProduct = _mapper.Map<ProductDTO>(product);
        return updatedProduct;
    }
}
