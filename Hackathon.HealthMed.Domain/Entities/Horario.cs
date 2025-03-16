using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Domain.Entities;

public class Horario : BaseEntity
{
    public Guid MedicoId { get; private set; }
    public Medico? Medico { get; private set; }
    public DateTime DataHorario { get; private set; }
    public eStatusHorario Status { get; private set; }
    public decimal Valor { get; private set; }

    public Horario()
    {
    }

    public Horario(Guid medicoId, DateTime dataHorario, eStatusHorario status, decimal valor)
    {
        MedicoId = medicoId;
        DataHorario = dataHorario;
        Status = status;
        Valor = valor;
    }
}
