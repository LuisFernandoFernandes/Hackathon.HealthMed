using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Hackathon.HealthMed.Agendar.Infra.Model;

namespace Hackathon.HealthMed.Agendar.Application.Interface;

public interface IAgendamentoService
{
    Task<ServiceResult<bool>> AgendarAsync(Agendamento agendamento);
}
