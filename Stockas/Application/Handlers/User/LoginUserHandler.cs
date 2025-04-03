using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using UserEntity = Stockas.Entities.User;
using Stockas.Models.DTOS.User;
using Stockas.Application.Services;
using Stockas.Application.Commands.User;

namespace Stockas.Application.Handlers.User;

public class LoginUserHandler(
    StockasContext context,
    IPasswordHasher<UserEntity> passwordHasher,
    ITokenService tokenService,
    ILogger<LoginUserHandler> logger)
    : IRequestHandler<LoginUserCommand, TokenResponseDTO>
{
    private readonly StockasContext _context = context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<LoginUserHandler> _logger = logger;

    public async Task<TokenResponseDTO> Handle(
    LoginUserCommand request,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing login for email: {Email}", request.LoginUserDTO.Email);
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.LoginUserDTO.Email, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user, user.Password, request.LoginUserDTO.Password);

        if (verificationResult != PasswordVerificationResult.Success)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenResponseDTO(token, refreshToken, new UserBriefResponseDTO
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email
        });
    }
}