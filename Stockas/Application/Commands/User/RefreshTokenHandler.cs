using MediatR;
using Stockas.Application.Services;
using Stockas.Entities;
using Stockas.Models.DTOS.User;
using System.Security.Claims;

namespace Stockas.Application.Commands.User
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

            var principal = tokenService.GetPrincipalFromExpiredToken(request.RefreshTokenRequestDTO.AccessToken);
            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await context.Users.FindAsync(new object[] { userId }, cancellationToken);

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

            var newToken = tokenService.GenerateToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await context.SaveChangesAsync(cancellationToken);

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
