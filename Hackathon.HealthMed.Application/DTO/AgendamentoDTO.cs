using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

public class AgendamentoDTO
{
    public Guid? Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid HorarioId { get; set; }
    public eStatusAgendamento Status { get; set; }
    public string? JustificativaCancelamento { get; set; }
}

public class AgendarConsultaDTO
{
    public Guid HorarioId { get; set; }
}
