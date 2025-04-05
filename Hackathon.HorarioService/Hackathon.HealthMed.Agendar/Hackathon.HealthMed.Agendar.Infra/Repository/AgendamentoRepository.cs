using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Hackathon.HealthMed.Agendar.Infra.Interface;
using Hackathon.HealthMed.Agendar.Infra.Repository.Context;

namespace Hackathon.HealthMed.Agendar.Infra.Repository;

public class AgendamentoRepository(AppDBContext _context) : IAgendamentoRepository
{
    public async Task Agendar(Agendamento agendamento)
    {
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
    }
}
