using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

internal class UsuarioDTO
{
    public Guid? Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public eTipoUsuario TipoUsuario { get; set; }
    public bool Ativo { get; set; }
}
