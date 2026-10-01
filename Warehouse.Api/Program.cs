using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Warehouse.Application.Abstractions;
using Warehouse.Application.UseCases;
using Warehouse.Application.UseCases;
using Warehouse.Infrastructure.Messaging;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Repositories;
using StackExchange.Redis;
using Warehouse.Api.Grpc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WarehouseDb")));

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ReceiveStockHandler>();
builder.Services.AddScoped<WithdrawStockHandler>();
builder.Services.AddScoped<MoveStockHandler>();
builder.Services.AddSingleton<IEventPublisher>(
    _ => new KafkaEventPublisher(builder.Configuration["Kafka:BootstrapServers"]!));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));

builder.Services.AddHostedService(sp => new StockEventConsumer(
    builder.Configuration["Kafka:BootstrapServers"]!,
    sp.GetRequiredService<IConnectionMultiplexer>(),
    sp.GetRequiredService<ILogger<StockEventConsumer>>()));
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGrpc();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<StockGrpcService>();

app.Run();
