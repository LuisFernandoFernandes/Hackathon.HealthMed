
using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IHorarioRepository
{
    Task<IEnumerable<Horario>> BuscarHorarioParaEdicao(Guid medicoId, Guid id, DateTime dataHorario);
    Task<bool> BuscarHorarioPorMedicoEData(Guid medicoId, DateTime dataHorario);
    Task<IEnumerable<Horario>> BuscarHorarios(Guid medicoId);
    Task Cadastrar(Horario horario);
    Task Editar(Horario horario);
}
