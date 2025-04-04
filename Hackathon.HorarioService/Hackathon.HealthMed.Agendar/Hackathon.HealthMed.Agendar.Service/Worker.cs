using Hackathon.HealthMed.Agendar.Infra.Messaging;

namespace Hackathon.HealthMed.Agendar.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMqConsumer _rabbitMqConsumer;

    public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer)
    {
        _logger = logger;
        _rabbitMqConsumer = rabbitMqConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        while (true)
        {
            try
            {
                await _rabbitMqConsumer.StartListeningAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro fatal no RabbitMQ Consumer");
            }

            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
        }
    }
}
