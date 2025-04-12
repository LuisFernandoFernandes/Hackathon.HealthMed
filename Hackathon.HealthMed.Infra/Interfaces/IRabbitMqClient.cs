namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IRabbitMqClient
{
    Task SendMessage(string message, string exchange);
}
