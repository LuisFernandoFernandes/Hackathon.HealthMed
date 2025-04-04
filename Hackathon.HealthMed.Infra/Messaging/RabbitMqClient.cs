using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace Hackathon.HealthMed.Infra.Messaging;


public class RabbitMqClient : IRabbitMqClient
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<RabbitMqClient> _logger;

    public RabbitMqClient(IConfiguration configuration, ILogger<RabbitMqClient> logger)
    {
        _logger = logger; // Armazena o logger
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");

        _connectionFactory = new ConnectionFactory()
        {
            HostName = rabbitMqConfig["HostName"],
            UserName = rabbitMqConfig["UserName"],
            Password = rabbitMqConfig["Password"]
        };

        _retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<AlreadyClosedException>()
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning($"Tentativa {retryAttempt} falhou. Tentando novamente em {timespan.Seconds} segundos. Erro: {exception.Message}");
                }
            );
    }

    public async Task SendMessage(string message, string exchange)
    {
        _logger.LogInformation("Iniciando envio da mensagem para RabbitMQ...");

        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Tentando estabelecer conexão com RabbitMQ...");

            await using var connection = await _connectionFactory.CreateConnectionAsync();
            _logger.LogInformation("Conexão com RabbitMQ estabelecida.");

            await using var channel = await connection.CreateChannelAsync();
            _logger.LogInformation($"Publicando mensagem na exchange '{exchange}'.");

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: "",
                body: body
            );

            _logger.LogInformation($"Mensagem enviada com sucesso para '{exchange}': {message}");
        });
    }
}

