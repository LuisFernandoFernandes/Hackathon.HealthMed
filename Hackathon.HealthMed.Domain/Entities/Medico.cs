using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Domain.Entities;

public class Medico : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string CRM { get; set; }
    public eEspecialidade Especialidade { get; set; }

    public Medico()
    {
    }

    public Medico(Guid usuarioId, string cRM, eEspecialidade especialidade)
    {
        UsuarioId = usuarioId;
        CRM = cRM;
        Especialidade = especialidade;
    }
}
