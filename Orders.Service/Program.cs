using Orders.Service.Domain;
using Orders.Service.Infrastructure;
using Orders.Service.UseCases.CreateOrder;

// регистируем все
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DbImitationContext>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
builder.Services.AddHostedService<Orders.Service.BackgroundServices.OutboxPublisher>();
builder.Services.AddHostedService<Orders.Service.BackgroundServices.PaymentEventsConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();