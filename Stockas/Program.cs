using Microsoft.EntityFrameworkCore;
using Stockas.Entities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Server & DbContext
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<StockasContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLDB");
    options.UseSqlServer(conString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
