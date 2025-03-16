using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Infra.Repository;

public class HorarioRepository(AppDBContext _context) : IHorarioRepository
{
    public async Task Cadastrar(Horario horario)
    {
        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();
    }

    public async Task Editar(Horario horario)
    {
        _context.Horarios.Update(horario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> BuscarHorarioPorMedicoEData(Guid medicoId, DateTime dataHorario)
    {
        return await _context.Horarios.AnyAsync(a => a.MedicoId == medicoId && a.DataHorario == dataHorario);
    }

    public async Task<IEnumerable<Horario>> BuscarHorarioParaEdicao(Guid medicoId, Guid id, DateTime dataHorario)
    {
        return await _context.Horarios.Where(a => a.MedicoId == medicoId &&
        (a.Id == id || (a.DataHorario == dataHorario && a.Id != id))).ToListAsync();
    }

    public async Task<IEnumerable<Horario>> BuscarHorarios(Guid medicoId, eStatusHorario? status = null)
    {
        var query = _context.Horarios.Where(a => a.MedicoId == medicoId && a.DataHorario > DateTime.UtcNow);

        query = status.HasValue ? query.Where(a => a.Status == status.Value) : query;

        return await query.ToListAsync();
    }
}
