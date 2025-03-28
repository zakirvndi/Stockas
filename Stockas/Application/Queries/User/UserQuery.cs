using MediatR;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Queries.User;

public class UserQuery : IRequest<UserResponseDTO?>
{
    public int UserId { get; set; }

    public UserQuery(int userId)
    {
        UserId = userId;
    }
}