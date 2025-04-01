using MediatR;

namespace Stockas.Application.Commands.User
{
    public class LogoutCommand : IRequest<Unit>
    {
        public int UserId { get; }

        public LogoutCommand(int userId)
        {
            UserId = userId;
        }
    }
}
