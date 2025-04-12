using Hackathon.HealthMed.Agendar.Application.DTO;
using Hackathon.HealthMed.Agendar.Application.Interface;
using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Hackathon.HealthMed.Agendar.Infra.Interface;
using Hackathon.HealthMed.Agendar.Infra.Model;
using Serilog;
using System.Text.Json;

namespace Hackathon.HealthMed.Agendar.Application.Service;

public class MensagemService(IAgendamentoService _agendamentoService, ILogger _logger) : IMessageProcessor
{
    public async Task<ServiceResult<bool>> ProcessMessageAsync(string message)
    {
        try
        {
            _logger.Information("Processing message: {Message}", message);
            var agendamentoDTO = JsonSerializer.Deserialize<AgendamentoDTO>(message);
            var result = await _agendamentoService.AgendarAsync(new Agendamento(agendamentoDTO!.PacienteId, agendamentoDTO!.HorarioId));
            _logger.Information("Message processed successfully: {Message}", message);
            return result;

        }
        catch (Exception e)
        {
            _logger.Error(e, "Error processing message: {Message}", message);
            return new ServiceResult<bool>(e);
        }
    }
}
