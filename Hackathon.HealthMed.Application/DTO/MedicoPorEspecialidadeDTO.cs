using Hackathon.HealthMed.Domain.Enums;

namespace Hackathon.HealthMed.Application.DTO;

public class BuscarMedicosRequestDTO
{
    public eEspecialidade? Especialidade { get; set; } // Opcional
}

public class BuscarMedicosResponseDTO
{
    public List<MedicoResponseDTO> Medicos { get; set; } = [];
}

public class MedicoResponseDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CRM { get; set; }
    public eEspecialidade Especialidade { get; set; }
}
