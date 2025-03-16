using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Infra.Repository;

public class AgendamentoRepository(AppDBContext _context) : IAgendamentoRepository
{
    public async Task Adicionar(Agendamento agendamento)
    {
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
    }

    public Task Editar(Agendamento agendamento)
    {
        if (_context.Entry(agendamento).State == EntityState.Detached)
        {
            _context.Agendamentos.Attach(agendamento);
            _context.Entry(agendamento).State = EntityState.Modified;
        }
        return _context.SaveChangesAsync();
    }

    public async Task<Agendamento?> BuscarPorId(Guid agendamentoId)
    {
        return await _context.Agendamentos.Where(a => a.Id == agendamentoId).FirstOrDefaultAsync();
    }
}
