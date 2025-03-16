using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;

namespace Hackathon.HealthMed.Infra.Repository;

public class AgendamentoRepository(AppDBContext _context) : IAgendamentoRepository
{
    public async Task Adicionar(Agendamento agendamento)
    {
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
    }
}
