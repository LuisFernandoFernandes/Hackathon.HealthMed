using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.DTO;

public class LoginRequestDTO
{
    public string Login { get; set; }
    public string Senha { get; set; }
    public eTipoUsuario TipoUsuario { get; set; }
}

public class LoginResponseDTO
{
    public string Token { get; set; }
}
