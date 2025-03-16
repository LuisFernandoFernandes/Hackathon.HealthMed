using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IAgendamentoRepository
{
    Task Adicionar(Agendamento agendamento);
    Task Editar(Agendamento agendamento);
    Task<Agendamento?> BuscarPorId(Guid agendamentoId);
}
