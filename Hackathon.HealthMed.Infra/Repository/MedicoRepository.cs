using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enums;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Infra.Repository;

public class MedicoRepository(AppDBContext _context) : IMedicoRepository
{
    public async Task<IEnumerable<Medico>> BuscarMedicos(eEspecialidade? especialidade)
    {
        return await _context.Medicos.Include(a => a.Usuario).Where(a => especialidade == null || a.Especialidade == especialidade).ToListAsync();
    }
}
