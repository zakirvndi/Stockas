using Stockas.Entities;
using System.Security.Claims;

namespace Stockas.Application.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);
}