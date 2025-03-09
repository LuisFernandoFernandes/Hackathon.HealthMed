using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

public class AtualizarStatusConsultaRequestDTO
{
    public Guid AgendamentoId { get; set; }
    public eStatusAgendamento NovoStatus { get; set; }
}

public class AgendarConsultaRequestDTO
{
    public Guid PacienteId { get; set; }
    public Guid HorarioId { get; set; }
}

public class AgendamentoResponseDTO
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid MedicoId { get; set; }
    public DateTime DataHorario { get; set; }
    public eStatusAgendamento Status { get; set; }
}

public class CancelamentoConsultaRequestDTO
{
    public Guid AgendamentoId { get; set; }
    public string Justificativa { get; set; }
}
