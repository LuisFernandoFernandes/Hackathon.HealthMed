using Hackathon.HealthMed.Agendar.Infra.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;

namespace Hackathon.HealthMed.Agendar.Infra.Messaging;


public class RabbitMqConsumer
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _filaConsummer;
    private readonly string _exchange;
    private readonly ILogger _logger;

    private static readonly Counter MessagesProcessed = Metrics.CreateCounter("rabbitmq_messages_processed", "Número de mensagens processadas com sucesso");
    private static readonly Counter MessagesFailed = Metrics.CreateCounter("rabbitmq_messages_failed", "Número de mensagens que falharam ao processar");
    private static readonly Histogram MessageProcessingDuration = Metrics.CreateHistogram("rabbitmq_message_processing_duration_seconds", "Duração do processamento das mensagens", new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(0.1, 0.1, 10)  // Exemplos de buckets de latência (ajuste conforme necessário)
    });

    public RabbitMqConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger logger)
    {
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");
        _connectionFactory = new ConnectionFactory()
        {
            HostName = rabbitMqConfig["HostName"] ?? throw new ArgumentNullException(nameof(configuration)),
            UserName = rabbitMqConfig["UserName"] ?? throw new ArgumentNullException(nameof(configuration)),
            Password = rabbitMqConfig["Password"] ?? throw new ArgumentNullException(nameof(configuration)),
            Port = Convert.ToInt32(rabbitMqConfig["Port"])
        };
        _serviceProvider = serviceProvider;
        _filaConsummer = rabbitMqConfig["QueueName"] ?? throw new ArgumentNullException(nameof(configuration));
        _exchange = rabbitMqConfig["Exchange"] ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger;
    }

    public void StartListeningAsync()
    {
        var retryPolicy = Policy
            .Handle<BrokerUnreachableException>() // Tenta novamente se o RabbitMQ não estiver acessível
            .Or<Exception>()                      // Ou se houver outra exceção
            .WaitAndRetry(
                retryCount: 5,                    // Número de tentativas
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(20 * attempt), // Intervalo entre tentativas
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    _logger.Error(exception, "Erro RabbitMQ: {Message}", exception.Message);
                    _logger.Information("Tentativa {RetryAttempt} falhou. Tentando novamente em {Timespan} segundos...", retryAttempt, timespan.Seconds);
                }
            );

        retryPolicy.Execute(async () =>
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _filaConsummer,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            await channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout, true, false);
            await channel.QueueBindAsync(_filaConsummer, _exchange, "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                _logger.Information("Mensagem recebida");
                var stopwatch = Stopwatch.StartNew();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                using var scope = _serviceProvider.CreateScope();
                var messageProcessor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();

                var resultadoProcessamento = await messageProcessor.ProcessMessageAsync(message);

                if (resultadoProcessamento.IsSuccess)
                    MessagesProcessed.Inc();
                else
                    MessagesFailed.Inc();

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                stopwatch.Stop();
                MessageProcessingDuration.Observe(stopwatch.Elapsed.TotalSeconds);
                _logger.Information("Mensagem processada Tempo em milliseconds {milliseconds}", stopwatch.Elapsed.TotalMilliseconds);
            };
            await channel.BasicConsumeAsync(queue: _filaConsummer, autoAck: false, consumer: consumer);

            _logger.Information("Escutando a fila {QueueName}", _filaConsummer);
            await Task.Delay(Timeout.Infinite);
        });




    }

    public async Task StartListeningAsync(CancellationToken stoppingToken)
    {
        _logger.Information("🚀 Iniciando consumo da fila RabbitMQ...");
        var retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(20 * attempt),
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    _logger.Error(exception, "Erro RabbitMQ: {Message}", exception.Message);
                    _logger.Information("Tentativa {RetryAttempt} falhou. Tentando novamente em {Timespan} segundos...", retryAttempt, timespan.Seconds);
                }
            );

        await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _filaConsummer, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout, true, false);
            await channel.QueueBindAsync(_filaConsummer, _exchange, "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body1 = ea.Body.ToArray();
                var message1 = Encoding.UTF8.GetString(body1);
                _logger.Information("Mensagem recebida: {Message}", message1);
                var stopwatch = Stopwatch.StartNew();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                using var scope = _serviceProvider.CreateScope();
                IMessageProcessor? messageProcessor = null;
                try
                {
                    messageProcessor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "❌ Não foi possível resolver IMessageProcessor");
                    throw;
                }


                var resultadoProcessamento = await messageProcessor.ProcessMessageAsync(message);

                if (resultadoProcessamento.IsSuccess)
                    MessagesProcessed.Inc();
                else
                    MessagesFailed.Inc();

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                stopwatch.Stop();
                MessageProcessingDuration.Observe(stopwatch.Elapsed.TotalSeconds);
                _logger.Information("Mensagem processada Tempo em milliseconds {milliseconds}", stopwatch.Elapsed.TotalMilliseconds);
            };

            await channel.BasicConsumeAsync(queue: _filaConsummer, autoAck: false, consumer: consumer);

            _logger.Information("Escutando a fila {QueueName}", _filaConsummer);

            // Mantém a execução contínua
            await Task.Delay(Timeout.Infinite, stoppingToken);
        });
    }


}

