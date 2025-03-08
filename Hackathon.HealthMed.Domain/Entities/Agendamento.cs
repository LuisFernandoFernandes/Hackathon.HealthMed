using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Domain.Entities;

public class Agendamento : BaseEntity
{
    public Guid PacienteId { get; private set; }
    public Paciente? Paciente { get; private set; }
    public Guid HorarioId { get; private set; }
    public Horario? Horario { get; private set; }
    public eStatusAgendamento Status { get; private set; }

    public Agendamento(Guid pacienteId, Guid horarioId, eStatusAgendamento status)
    {
        PacienteId = pacienteId;
        HorarioId = horarioId;
        Status = status;
    }

    public void AtualizarStatus(eStatusAgendamento novoStatus)
    {
        Status = novoStatus;
    }
}
