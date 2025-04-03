using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.User;
using Stockas.Application.Services.Token;
using Stockas.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Stockas.Application.Handlers.User
{
    public class LogoutHandler(
        StockasContext context,
        ITokenBlacklistService tokenBlacklistService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LogoutHandler> logger)
        : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly StockasContext _context = context;
        private readonly ITokenBlacklistService _tokenBlacklistService = tokenBlacklistService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<LogoutHandler> _logger = logger;

        public async Task<Unit> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initiating logout for user ID: {UserId}", request.UserId);

            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("User not found during logout (ID: {UserId})", request.UserId);
                return Unit.Value;
            }

            // Get the current access token from the Authorization header
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(accessToken);
                    var remainingLife = jwtToken.ValidTo - DateTime.UtcNow;

                    if (remainingLife > TimeSpan.Zero)
                    {
                        await _tokenBlacklistService.BlacklistTokenAsync(accessToken, remainingLife);
                        _logger.LogInformation("Access token blacklisted for user ID: {UserId}", request.UserId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to blacklist token for user ID: {UserId}", request.UserId);
                }
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Logout completed for user ID: {UserId}", request.UserId);

            return Unit.Value;
        }
    }
}