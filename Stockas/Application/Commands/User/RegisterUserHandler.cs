using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stockas.Entities;
using UserEntity = Stockas.Entities.User;
using Stockas.Models.DTOS.User;
using Stockas.Application.Services;
using FluentValidation;

namespace Stockas.Application.Commands.User;

public class RegisterUserHandler(
    StockasContext context,
    IPasswordHasher<UserEntity> passwordHasher,
    ITokenService tokenService,
    ILogger<RegisterUserHandler> logger)
    : IRequestHandler<RegisterUserCommand, TokenResponseDTO>
{

    private readonly StockasContext _context = context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RegisterUserHandler> _logger = logger;

    public async Task<TokenResponseDTO> Handle(
    RegisterUserCommand request,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing registration for email: {Email}", request.RegisterUserDTO.Email);
        var user = new UserEntity
        {
            Name = request.RegisterUserDTO.Name,
            Email = request.RegisterUserDTO.Email,
            Password = passwordHasher.HashPassword(null!, request.RegisterUserDTO.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync(cancellationToken);

        return new TokenResponseDTO(
            token,
            refreshToken,
            new UserBriefResponseDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email
            });
    }
}