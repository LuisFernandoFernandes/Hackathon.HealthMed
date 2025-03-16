
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IHorarioRepository
{
    Task<IEnumerable<Horario>> BuscarHorarioParaEdicao(Guid medicoId, Guid id, DateTime dataHorario);
    Task<bool> BuscarHorarioPorMedicoEData(Guid medicoId, DateTime dataHorario);
    Task<IEnumerable<Horario>> BuscarHorarios(Guid medicoId, eStatusHorario? Status);
    Task<Horario?> BuscarHorarioPorIdEStatus(Guid horarioId, eStatusHorario Status);
    Task Cadastrar(Horario horario);
    Task Editar(Horario horario);
}
