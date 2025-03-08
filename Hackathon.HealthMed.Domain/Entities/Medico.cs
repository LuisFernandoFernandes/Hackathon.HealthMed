using Hackathon.HealthMed.Domain.Enums;

namespace Hackathon.HealthMed.Domain.Entities;

public class Medico : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string CRM { get; set; }
    public eEspecialidade Especialidade { get; set; }

    public Medico(Guid id, Guid usuarioId, string cRM, eEspecialidade especialidade)
    {
        Id = id;
        UsuarioId = usuarioId;
        CRM = cRM;
        Especialidade = especialidade;
    }
}
