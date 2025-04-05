using Hackathon.HealthMed.Agendar.Domain.Enum;

namespace Hackathon.HealthMed.Agendar.Application.DTO;

public class AgendamentoDTO
{
    public Guid PacienteId { get; init; }
    public Guid HorarioId { get; init; }

}
