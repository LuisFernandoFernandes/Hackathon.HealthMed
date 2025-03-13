namespace Hackathon.HealthMed.Domain.Entities;

public class Paciente : BaseEntity
{
    public Guid UsuarioId { get; private set; }
    public Usuario? Usuario { get; private set; }
    public string Cpf { get; private set; }

    public Paciente()
    {
    }

    public Paciente(Guid usuarioId, string cpf)
    {
        UsuarioId = usuarioId;
        Cpf = cpf;
    }
}
