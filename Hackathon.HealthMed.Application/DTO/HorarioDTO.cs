using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

public class HorarioDTO
{
    public Guid? Id { get; set; }
    public Guid MedicoId { get; set; }
    public DateTime DataHorario { get; set; }
    public eStatusHorario Status { get; set; }
    public decimal Valor { get; set; }
}
