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
        SenhaHash = HashSenha(novaSenha);
    }

    private string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(hashedBytes);
    }

    public void DesativarUsuario()
    {
        Ativo = false;
    }
}

