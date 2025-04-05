using Hackathon.HealthMed.Agendar.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.HealthMed.Agendar.Domain.Entitites;

public class Agendamento
{
    public Guid Id { get; private set; }
    public Guid PacienteId { get; private set; }
    public Guid HorarioId { get; private set; }
    public eStatusAgendamento Status { get; private set; } = eStatusAgendamento.Pendente;
    public string? JustificativaCancelamento { get; private set; }
    public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;

    public Agendamento(Guid pacienteId, Guid horarioId)
    {
        Id = Guid.NewGuid();
        PacienteId = pacienteId;
        HorarioId = horarioId;
    }
}
