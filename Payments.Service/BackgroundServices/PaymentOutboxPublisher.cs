using Confluent.Kafka;
using Payments.Service.Infrastructure;

namespace Payments.Service.BackgroundServices
{
    // продюссер кафка
    public class PaymentOutboxPublisher : BackgroundService
    {
        private readonly DbPaymentImitationContext _dbContext;
        private readonly ILogger<PaymentOutboxPublisher> _logger;
        private readonly IProducer<Null, string> _producer;

        public PaymentOutboxPublisher(DbPaymentImitationContext dbContext, ILogger<PaymentOutboxPublisher> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092",
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        // цикл продюссера
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutbox();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing payment events");
                }
                await Task.Delay(100, stoppingToken); // Проверяем каждые 2 сек
            }
        }
        // обработка сообщений из outbox
        private async Task ProcessOutbox()
        {
            var messages = _dbContext.GetUnprocessedMessages();

            foreach (var msg in messages)
            {
                await _producer.ProduceAsync("payment-events", new Message<Null, string> 
                { 
                    Value = msg.Content 
                });

                _logger.LogInformation($"[Payments] Published event {msg.Type}");
                
                _dbContext.ExecuteTransaction(() =>
                {
                    var dbMsg = _dbContext.OutboxMessages.FirstOrDefault(m => m.Id == msg.Id);
                    if (dbMsg != null) dbMsg.ProcessedDate = DateTime.UtcNow;
                });
            }
        }
    }
}