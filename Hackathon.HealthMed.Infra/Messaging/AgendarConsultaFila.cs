using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hackathon.HealthMed.Infra.Messaging;

public class AgendarConsultaFila(IRabbitMqClient _rabbitMqClient, IConfiguration _configuration) : IAgendarConsultaFila
{
    public async Task AgendarAsync(Agendamento agendamento)
    {
        var rabbitMqConfig = _configuration.GetSection("RabbitMq");
        var mensagem = JsonSerializer.Serialize(agendamento, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        await _rabbitMqClient.SendMessage(mensagem, rabbitMqConfig["ExchangeAgendar"]);
    }
}
