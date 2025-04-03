using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.User;
using Stockas.Application.Queries.User;
using Stockas.Application.Services.Token;
using Stockas.Entities;
using Stockas.Models.DTOS.User;
using System.Security.Claims;

namespace Stockas.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly StockasContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        ITokenBlacklistService tokenBlacklistService,
        StockasContext context,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _tokenBlacklistService = tokenBlacklistService;
        _context = context;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        _logger.LogInformation("Login endpoint called");
        try
        {
            var result = await _mediator.Send(new LoginUserCommand(dto));
            _logger.LogInformation("Login endpoint completed successfully");
            return Ok(new { result.Token, result.User });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login endpoint error");
            throw;
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);
        try
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));
            _logger.LogInformation("Successful registration for user ID: {UserId}", result.User.UserId);
            return Ok(new { result.Token, result.User });
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Failed registration attempt for email: {Email}", dto.Email);
            throw;
        }
    }

    [HttpGet("profile/{userId:int}")]
    [Authorize]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Forbid();
        }

        if (currentUserId != userId)
        {
            return Forbid();
        }

        var user = await _mediator.Send(new UserQuery(userId));
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO dto)
    {
        _logger.LogInformation("Token refresh attempt");
        var result = await _mediator.Send(new RefreshTokenCommand(dto));
        _logger.LogInformation("Successful token refresh for user");
        return Ok(new { result.Token, result.RefreshToken, result.User });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Logout endpoint called");
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _mediator.Send(new LogoutCommand(userId));
            _logger.LogInformation("Logout endpoint completed successfully");
            return Ok(new { Message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout endpoint error");
            throw;
        }
    }
}