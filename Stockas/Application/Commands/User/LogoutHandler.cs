using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;

namespace Stockas.Application.Commands.User
{
    public class LogoutHandler(
    StockasContext context,
    ILogger<LogoutHandler> logger)
    : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly StockasContext _context = context;
        private readonly ILogger<LogoutHandler> _logger = logger;

        public async Task<Unit> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initiating logout for user ID: {UserId}", request.UserId);
            var user = await context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("User not found during logout (ID: {UserId})", request.UserId);
                return Unit.Value;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Logout completed for user ID: {UserId}", request.UserId);

            return Unit.Value;
        }
    }
}
