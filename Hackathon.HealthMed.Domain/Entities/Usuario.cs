using BCrypt.Net;
using Hackathon.HealthMed.Domain.Enum;
using System.Security.Cryptography;
using System.Text;

namespace Hackathon.HealthMed.Domain.Entities;
public class Usuario : BaseEntity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public eTipoUsuario TipoUsuario { get; private set; }
    public bool Ativo { get; private set; } = true;

    public Usuario(string nome, string email, string senhaHash, eTipoUsuario tipoUsuario)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        TipoUsuario = tipoUsuario;
    }

    public void AlterarSenha(string novaSenha)
    {
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
    }

    public bool ValidarSenha(string senha)
    {
        return BCrypt.Net.BCrypt.Verify(senha, SenhaHash);
    }

    public void DesativarUsuario()
    {
        Ativo = false;
    }
}

