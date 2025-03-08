namespace Hackathon.HealthMed.Domain.Enum;

/// <summary>
/// Esse enum poderia ter sido substituido por um booleano, mas para extensões futuras um enum é mais apropriedado.
/// Poderia incluir por exemplo uma secretária ou um administrador.
/// </summary>
public enum eTipoUsuario
{
    Medico,
    Paciente
}
