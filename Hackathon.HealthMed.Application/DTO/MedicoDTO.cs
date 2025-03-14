using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enums;

namespace Hackathon.HealthMed.Application.DTO;

public class MedicoDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CRM { get; set; }
    public eEspecialidade Especialidade { get; set; }
}
