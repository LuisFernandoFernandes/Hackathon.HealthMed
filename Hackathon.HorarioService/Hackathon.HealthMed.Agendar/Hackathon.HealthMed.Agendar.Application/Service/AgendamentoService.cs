using Hackathon.HealthMed.Agendar.Application.Interface;
using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Hackathon.HealthMed.Agendar.Infra.Interface;
using Hackathon.HealthMed.Agendar.Infra.Model;
using Microsoft.Extensions.Logging;

namespace Hackathon.HealthMed.Agendar.Application.Service;

public class AgendamentoService(IAgendamentoRepository _agendamentoRepository, ILogger<AgendamentoService> _logger) : IAgendamentoService
{

    public async Task<ServiceResult<bool>> AgendarAsync(Agendamento agendamento)
    {
        try
        {
            await _agendamentoRepository.Agendar(agendamento);
            _logger.LogInformation("Agendamento realizado com sucesso: {Agendamento}", agendamento);

            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao agendar: {Message}", ex.Message);
            return new ServiceResult<bool>(ex);
        }
    }
}
