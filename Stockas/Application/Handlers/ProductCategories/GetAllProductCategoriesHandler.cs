﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Queries;
using Stockas.Entities;
using Stockas.Models.DTOS;

public class GetAllProductCategoriesQueryHandler : IRequestHandler<GetAllProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly StockasContext _context;
    private readonly IMapper _mapper;

    public GetAllProductCategoriesQueryHandler(StockasContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductCategoryDto>> Handle(GetAllProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.ProductCategories
            .Where(c => c.UserId == request.UserId) 
            .ToListAsync(cancellationToken);

        if (categories == null || !categories.Any())
        {
            throw new KeyNotFoundException("No product categories found for this user.");
        }

        return _mapper.Map<List<ProductCategoryDto>>(categories);
    }

}
