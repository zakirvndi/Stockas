using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDTO>
{
    private readonly StockasContext _context;
    private readonly IMapper _mapper;

    public CreateProductHandler(StockasContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            bool isExist = await _context.Products.AnyAsync(p => p.ProductName == request.ProductName && p.UserId == request.UserId, cancellationToken);
            if (isExist)
                throw new Exception("You already have a product with the same name.");

            var product = _mapper.Map<Product>(request);
            product.UserId = request.UserId; 

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            var productWithCategory = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId, cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected error: {ex.Message}", ex);
        }
    }
}
