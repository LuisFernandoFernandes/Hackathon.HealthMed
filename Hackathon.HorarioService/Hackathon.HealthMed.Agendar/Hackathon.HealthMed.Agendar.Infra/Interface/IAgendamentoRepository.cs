using Hackathon.HealthMed.Agendar.Domain.Entitites;

namespace Hackathon.HealthMed.Agendar.Infra.Interface;

public interface IAgendamentoRepository
{
    Task Agendar(Agendamento agendamento);
}
