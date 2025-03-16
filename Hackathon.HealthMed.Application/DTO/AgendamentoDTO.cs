using Hackathon.HealthMed.Domain.Enum;
using System.ComponentModel;

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

public class ConfirmarAgendamentoDTO
{
    public Guid AgendamentoId { get; set; }
    public bool Aceitar { get; set; }
    public string? Justificativa { get; set; }
}

public class CancelarAgendamentoPacienteDTO
{
    public Guid AgendamentoId { get; set; }
    public string Justificativa { get; set; } = string.Empty;
}

public class CancelarAgendamentoMedicoDTO
{
    public Guid AgendamentoId { get; set; }

    [DefaultValue(null)]
    public string? Justificativa { get; set; }
}


