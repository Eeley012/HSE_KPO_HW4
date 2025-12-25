using Confluent.Kafka;
using Payments.Service.Infrastructure;
using Payments.Service.Messages;
using System.Text.Json;

namespace Payments.Service.BackgroundServices
{
    // слушатель сообщений кафка 
    public class OrderEventsConsumer : BackgroundService
    {
        private readonly DbPaymentImitationContext _dbContext;
        private readonly ILogger<OrderEventsConsumer> _logger;
        private readonly IConsumer<Null, string> _consumer;

        public OrderEventsConsumer(DbPaymentImitationContext dbContext, ILogger<OrderEventsConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092",
                GroupId = "payments-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }
        // уикл чтения сообщений
        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("orders-events");

            _logger.LogInformation("Payments Consumer started listening to 'orders-events'...");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    
                    if (consumeResult == null) continue;

                    var messageContent = consumeResult.Message.Value;
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(messageContent);

                    if (orderEvent != null)
                    {
                         HandleMessage(orderEvent, consumeResult.Message.Timestamp.UnixTimestampMs);
                    }
                    _consumer.Commit(consumeResult);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
            }
            
            _consumer.Close();
        }
        // обрабатываем сообщения
        private void HandleMessage(OrderCreatedEvent orderEvent, long messagePseudoId)
        {
            var idempotencyKey = orderEvent.OrderId;

            _dbContext.ExecuteTransaction(() =>
            {
                if (_dbContext.InboxMessages.Any(m => m.Id == idempotencyKey))
                {
                    _logger.LogInformation($"Message for Order {orderEvent.OrderId} already processed.");
                    return;
                }

                var account = _dbContext.Accounts.FirstOrDefault(a => a.UserId == orderEvent.UserId);
                
                object resultEvent;
                if (account == null)
                {
                    _logger.LogWarning($"Account not found for user {orderEvent.UserId}");
                    resultEvent = new PaymentFailedEvent 
                    { 
                        OrderId = orderEvent.OrderId, 
                        Reason = "Account not found" 
                    };
                }
                else
                {
                    if (account.TryWithdraw(orderEvent.Amount))
                    {
                        _logger.LogInformation($"Successfully withdrew {orderEvent.Amount} from user {orderEvent.UserId}");
                        resultEvent = new PaymentSucceededEvent 
                        { 
                            OrderId = orderEvent.OrderId 
                        };
                    }
                    else
                    {
                        _logger.LogWarning($"Insufficient funds for user {orderEvent.UserId}");
                        resultEvent = new PaymentFailedEvent 
                        { 
                            OrderId = orderEvent.OrderId, 
                            Reason = "Insufficient funds" 
                        };
                    }
                }
                _dbContext.InboxMessages.Add(new InboxMessage 
                { 
                    Id = idempotencyKey, 
                    ProcessedAt = DateTime.UtcNow 
                });
                _dbContext.OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = resultEvent.GetType().Name,
                    Content = JsonSerializer.Serialize(resultEvent),
                    OccurredOn = DateTime.UtcNow
                });
            });
        }
    }
}