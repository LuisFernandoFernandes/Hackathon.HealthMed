
namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IPacienteRepository
{
    Task<Guid> BuscarPacientePorUsuarioId(Guid guid);
}
