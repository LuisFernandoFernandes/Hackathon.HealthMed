using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Infra.Repository;

public class PacienteRepository(AppDBContext _context) : IPacienteRepository
{
    public async Task<Guid> BuscarPacientePorUsuarioId(Guid guid)
    {
        return await _context.Pacientes.Where(a => a.UsuarioId == guid).Select(a => a.Id).FirstOrDefaultAsync();
    }
}
