using Confluent.Kafka;
using Orders.Service.Infrastructure;

namespace Orders.Service.BackgroundServices
{
    // класс-продюссер сообщений для outbox
    public class OutboxPublisher : BackgroundService
    {
        private readonly DbImitationContext _dbContext;
        private readonly ILogger<OutboxPublisher> _logger;
        private readonly IProducer<Null, string> _producer;

        public OutboxPublisher(DbImitationContext dbContext, ILogger<OutboxPublisher> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092",
            };
            
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        // уикл работы продюссера сообщений
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Publisher started working...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessages();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing outbox");
                }
                
                await Task.Delay(100, stoppingToken);
            }
        }
        private async Task ProcessOutboxMessages()
        {
            var messagesToProcess = _dbContext.GetUnprocessedMessages();

            if (messagesToProcess.Any())
            {
                _logger.LogInformation($"Found {messagesToProcess.Count} messages to publish.");
            }

            foreach (var message in messagesToProcess)
            {
                var kafkaMessage = new Message<Null, string>
                {
                    Value = message.Content
                };
                
                await _producer.ProduceAsync("orders-events", kafkaMessage);
                _logger.LogInformation($"Published message {message.Id} to Kafka.");

                _dbContext.ExecuteTransaction(() =>
                {
                    var msgInDb = _dbContext.OutboxMessages.FirstOrDefault(m => m.Id == message.Id);
                    if (msgInDb != null)
                    {
                        msgInDb.ProcessedDate = DateTime.UtcNow;
                    }
                });
            }
        }
    }
}