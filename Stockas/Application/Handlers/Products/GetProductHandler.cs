using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class GetProductHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDTO>>
{
    private readonly StockasContext _context;
    private readonly IMapper _mapper;

    public GetProductHandler(StockasContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDTO>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        query = request.OrderBy?.ToLower() switch
        {
            "date" => request.OrderDesc ? query.OrderByDescending(p => p.InputDate) : query.OrderBy(p => p.InputDate),
            "stock" => request.OrderDesc ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity),
            "sellingprice" => request.OrderDesc ? query.OrderByDescending(p => p.SellingPrice) : query.OrderBy(p => p.SellingPrice),
            "buyingprice" => request.OrderDesc ? query.OrderByDescending(p => p.PurchasePrice) : query.OrderBy(p => p.PurchasePrice),
            _ => request.OrderDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
        };

       
        if (request.GroupByCategory)
        {
            query = query.OrderBy(p => p.Category.CategoryName);
        }

      
        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        if (products == null || !products.Any())
        {
            throw new KeyNotFoundException("No product found.");
        }


        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }
}
