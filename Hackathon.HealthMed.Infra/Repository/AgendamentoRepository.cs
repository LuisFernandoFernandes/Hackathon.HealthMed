using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
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

    public async Task<IEnumerable<Agendamento>> ConsultarAgendamentosPorUsuario(eTipoUsuario usuarioTipo, Guid usuarioTipoId)
    {
        var hoje = DateTime.Today;

        switch (usuarioTipo)
        {
            case eTipoUsuario.Medico:

                return await _context.Agendamentos.Where(a => a.Horario != null && a.Horario.MedicoId == usuarioTipoId && a.Horario.DataHorario >= hoje).ToListAsync();

            case eTipoUsuario.Paciente:

                return await _context.Agendamentos.Where(a => a.Horario != null && a.PacienteId == usuarioTipoId && a.Horario.DataHorario >= hoje).ToListAsync();

            default:
                return await Task.FromResult(Enumerable.Empty<Agendamento>());
        }
    }
}
