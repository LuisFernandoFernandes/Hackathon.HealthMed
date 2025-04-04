using Hackathon.HealthMed.Agendar.Domain.Enum;

namespace Hackathon.HealthMed.Agendar.Application.DTO;

public class AgendamentoDTO
{
    public Guid? Id { get; init; }
    public Guid PacienteId { get; init; }
    public Guid HorarioId { get; init; }
    public eStatusAgendamento Status { get; init; } = eStatusAgendamento.Pendente;
    public string? JustificativaCancelamento { get; init; } = null;
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;

}
