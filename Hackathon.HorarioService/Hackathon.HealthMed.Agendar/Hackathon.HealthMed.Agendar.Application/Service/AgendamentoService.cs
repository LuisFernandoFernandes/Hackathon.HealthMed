using Hackathon.HealthMed.Agendar.Application.Interface;
using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Hackathon.HealthMed.Agendar.Infra.Interface;
using Hackathon.HealthMed.Agendar.Infra.Model;

namespace Hackathon.HealthMed.Agendar.Application.Service;

public class AgendamentoService(IAgendamentoRepository _agendamentoRepository) : IAgendamentoService
{
    public async Task<ServiceResult<bool>> AgendarAsync(Agendamento agendamento)
    {
        try
        {
            await _agendamentoRepository.Agendar(agendamento);

            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new ServiceResult<bool>(ex);
        }
    }
}
