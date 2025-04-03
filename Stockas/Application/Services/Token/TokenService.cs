using Microsoft.IdentityModel.Tokens;
using Stockas.Application.Services.Token;
using Stockas.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Stockas.Application.Services;

public class TokenService : ITokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly ILogger<TokenService> _logger;
    private readonly SymmetricSecurityKey _securityKey;

    public TokenService(
        string key,
        string issuer,
        string audience,
        ITokenBlacklistService tokenBlacklistService,
        ILogger<TokenService> logger)
    {
        _key = key;
        _issuer = issuer;
        _audience = audience;
        _tokenBlacklistService = tokenBlacklistService;
        _logger = logger;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
    }

    public string GenerateToken(User user)
    {
        _logger.LogInformation("Generating token for user ID: {UserId}", user.UserId);

        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _tokenBlacklistService.IsTokenBlacklistedAsync(token);
    }

    public async Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token)
    {
        _logger.LogInformation("Validating expired token");

        if (await _tokenBlacklistService.IsTokenBlacklistedAsync(token))
        {
            throw new SecurityTokenException("Token has been revoked");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            throw new SecurityTokenException("Token validation failed", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        if (await _tokenBlacklistService.IsTokenBlacklistedAsync(token))
        {
            return false;
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}