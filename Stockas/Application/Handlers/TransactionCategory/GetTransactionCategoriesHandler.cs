using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Queries.TransactionCategory;
using Stockas.Entities;
using Stockas.Models.DTOS;
using System.Security.Claims;

namespace Stockas.Application.Handlers.TransactionCategory
{
    public class GetTransactionCategoriesHandler : IRequestHandler<GetTransactionCategoriesQuery, List<TransactionCategoryDTO>>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTransactionCategoriesHandler(
            StockasContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TransactionCategoryDTO>> Handle(GetTransactionCategoriesQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }

            var categories = await _context.TransactionCategories
                .Where(c => c.UserId == userId)
                .ToListAsync(cancellationToken);

            if (!categories.Any())
            {
                throw new KeyNotFoundException("No transaction categories found.");
            }

            return _mapper.Map<List<TransactionCategoryDTO>>(categories);
        }
    }
}
