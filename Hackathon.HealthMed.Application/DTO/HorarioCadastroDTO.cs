using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

public class CadastroHorarioRequestDTO
{
    public DateTime DataHorario { get; set; }
    public decimal Valor { get; set; }
    public eStatusHorario Status { get; set; }
}

public class CadastroHorarioResponseDTO
{
    public Guid Id { get; set; }
    public DateTime DataHorario { get; set; }
    public decimal Valor { get; set; }
    public eStatusHorario Status { get; set; }
}

public class BuscarHorariosMedicoRequestDTO
{
    public Guid MedicoId { get; set; }
}

public class BuscarHorariosMedicoResponseDTO
{
    public Guid MedicoId { get; set; }
    public string NomeMedico { get; set; }
    public List<BuscarHorarioResponseDTO> HorariosDisponiveis { get; set; } = [];
}

public class BuscarHorarioResponseDTO
{
    public Guid Id { get; set; }
    public DateTime DataHorario { get; set; }
    public decimal Valor { get; set; }
    public eStatusHorario Status { get; set; }
}

