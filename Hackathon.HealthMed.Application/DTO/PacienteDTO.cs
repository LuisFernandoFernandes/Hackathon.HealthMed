namespace Hackathon.HealthMed.Application.DTO;

public class PacienteDTO
{
    public Guid? Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Cpf { get; set; }
}
