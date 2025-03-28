using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Queries.User;

public class UserQueryHandler(StockasContext context)
    : IRequestHandler<UserQuery, UserResponseDTO?>
{
    public async Task<UserResponseDTO?> Handle(
        UserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(u => u.UserId == request.UserId)
            .Select(u => new UserResponseDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}