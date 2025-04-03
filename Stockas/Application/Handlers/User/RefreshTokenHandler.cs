using MediatR;
using Stockas.Application.Commands.User;
using Stockas.Application.Services;
using Stockas.Entities;
using Stockas.Models.DTOS.User;
using System.Security.Claims;

namespace Stockas.Application.Handlers.User
{
    public class RefreshTokenHandler(
    StockasContext context,
    ITokenService tokenService,
    ILogger<RefreshTokenHandler> logger)
    : IRequestHandler<RefreshTokenCommand, TokenResponseDTO>
    {

        private readonly StockasContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<RefreshTokenHandler> _logger = logger;

        public async Task<TokenResponseDTO> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing token refresh");
            if (string.IsNullOrEmpty(request.RefreshTokenRequestDTO.AccessToken) ||
                string.IsNullOrEmpty(request.RefreshTokenRequestDTO.RefreshToken))
            {
                throw new UnauthorizedAccessException("Token and refresh token are required");
            }

            var principal = await _tokenService.GetPrincipalFromExpiredTokenAsync(request.RefreshTokenRequestDTO.AccessToken);
            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            if (user.RefreshToken != request.RefreshTokenRequestDTO.RefreshToken)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expired");
            }

            var newToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync(cancellationToken);

            return new TokenResponseDTO(
                newToken,
                newRefreshToken,
                new UserBriefResponseDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email
                });
        }
    }
}
