using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stockas.Application.Behaviors;
using Stockas.Application.Middleware;
using Stockas.Application.Services;
using Stockas.Application.Services.Token;
using Stockas.Entities;
using Stockas.Models.DTOS;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Konfigurasi Serilog Sink File
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "logs/Log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Konfigurasi SQL Server & DbContext
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<StockasContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLDB");
    options.UseSqlServer(conString);
});

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(ProductCategoryDto).Assembly);

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Konfigurasi Authentication & JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };
    });

// Konfigurasi Redis untuk Token Blacklist
if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Redis")))
{
    throw new ApplicationException("Redis connection string is missing in configuration");
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "JWT_Blacklist_";
});

// Register Token Services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ITokenBlacklistService, RedisTokenBlacklistService>();

builder.Services.AddSingleton<ITokenService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new TokenService(
        config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"),
        config["Jwt:Issuer"] ?? "StockasAPI",
        config["Jwt:Audience"] ?? "StockasClient",
        provider.GetRequiredService<ITokenBlacklistService>(),
        provider.GetRequiredService<ILogger<TokenService>>());
});

var app = builder.Build();

// Middleware Global Error Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
