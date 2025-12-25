using Confluent.Kafka;
using Orders.Service.Domain;
using Orders.Service.Messages;
using System.Text.Json;

namespace Orders.Service.BackgroundServices
{
    // читатель сообщений в kafka
    public class PaymentEventsConsumer : BackgroundService
    {
        private readonly ILogger<PaymentEventsConsumer> _logger;
        
        private readonly IServiceProvider _serviceProvider;
        
        // читатель сообщений
        private readonly IConsumer<Null, string> _consumer;

        public PaymentEventsConsumer(IServiceProvider serviceProvider, ILogger<PaymentEventsConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092",
                GroupId = "orders-service-group",
                
                AutoOffsetReset = AutoOffsetReset.Earliest,
                
                EnableAutoCommit = false
            };
            
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        // цикл работы читателя сообщений
        private async Task StartConsumerLoop(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("payment-events");
            
            _logger.LogInformation("Orders Service started lestening to 'payment-events'...");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    
                    if (result == null) continue;

                    var jsonMessage = result.Message.Value;
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                        await ProcessMessage(jsonMessage, repository);
                    }
                    _consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured while processing payment message");
                    await Task.Delay(1000, stoppingToken);
                }
            }
            
            _consumer.Close();
        }

        // обрабатывам содержание сообщения
        private async Task ProcessMessage(string json, IOrderRepository repository)
        {
            // сообщение об ошибка оплаты
            if (json.Contains("Reason")) 
            {
                var failEvent = JsonSerializer.Deserialize<PaymentFailedEvent>(json);
                if (failEvent != null)
                {
                    var order = await repository.GetByIdAsync(failEvent.OrderId);
                    if (order != null)
                    {
                        order.MarkAsFailed(); 
                        await repository.SaveAsync(order); 
                        
                        _logger.LogInformation($"Order {order.Id} is failed.\nReason: {failEvent.Reason}");
                    }
                }
            }
            else // сообщение об успешное оплате
            {
                var successEvent = JsonSerializer.Deserialize<PaymentSucceededEvent>(json);
                if (successEvent != null)
                {
                    var order = await repository.GetByIdAsync(successEvent.OrderId);
                    if (order != null)
                    {
                        order.MarkAsPaid();
                        
                        await repository.SaveAsync(order);
                        
                        _logger.LogInformation($"Order {order.Id}  is successfully paid.");
                    }
                }
            }
        }
    }
}