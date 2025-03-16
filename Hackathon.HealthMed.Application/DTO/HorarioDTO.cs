using Hackathon.HealthMed.Domain.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.HealthMed.Application.DTO;

public class HorarioDTO
{
    public Guid Id { get; set; }
    public DateTime DataHorario { get; set; }
    public eStatusHorario Status { get; set; }
    public decimal Valor { get; set; }
}

public class CadastrarHorarioDTO
{
    [DefaultValue(null)]
    public Guid? MedicoId { get; set; }
    public DateTime DataHorario { get; set; }
    public decimal Valor { get; set; }
}

public class EditarHorarioDTO
{
    public Guid Id { get; set; }
    public DateTime DataHorario { get; set; }
    public eStatusHorario Status { get; set; }
    public decimal Valor { get; set; }
}