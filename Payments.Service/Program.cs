using Payments.Service.Domain;
using Payments.Service.Infrastructure;
using Payments.Service.UseCases.CreateAccount;

// добавляем все зависимости в контейнер
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DbPaymentImitationContext>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICreateAccountUseCase, CreateAccountUseCase>();
builder.Services.AddHostedService<Payments.Service.BackgroundServices.PaymentOutboxPublisher>();
builder.Services.AddHostedService<Payments.Service.BackgroundServices.OrderEventsConsumer>();
builder.Services.AddScoped<Payments.Service.UseCases.Deposit.IDepositUseCase, Payments.Service.UseCases.Deposit.DepositUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();