using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stockas.Application.Middleware;
using Stockas.Application.Services;
using Stockas.Application.Services.Token;
using Stockas.Entities;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();

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

if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Redis")))
{
    throw new ApplicationException("Redis connection string is missing in configuration");
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "JWT_Blacklist_";
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


// Configure SQL Server & DbContext
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<StockasContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLDB");
    options.UseSqlServer(conString);
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
