namespace Hackathon.HealthMed.Application.DTO;

public class LoginMedicoRequestDTO
{
    public string CRM { get; set; }
    public string Senha { get; set; }
}

public class LoginPacienteRequestDTO
{
    public string EmailOuCPF { get; set; }
    public string Senha { get; set; }
}

public class LoginResponseDTO
{
    public string Token { get; set; }
}
