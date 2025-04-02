using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IAgendamentoRepository
{
    Task Adicionar(Agendamento agendamento);
    Task Editar(Agendamento agendamento);
    Task<Agendamento?> BuscarPorId(Guid agendamentoId);
    Task<IEnumerable<Agendamento>> ConsultarAgendamentosPorUsuario(eTipoUsuario usuarioTipo, Guid usuarioTipoId);
}
