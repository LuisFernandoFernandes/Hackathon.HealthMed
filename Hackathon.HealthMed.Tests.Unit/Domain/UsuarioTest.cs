using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Tests.Unit.Domain.Entities;

public class UsuarioTests
{
    [Fact]
    public void Constructor_WithParameters_SetsProperties()
    {
        // Arrange
        var nome = "Fulano";
        var email = "fulano@teste.com";
        var senha = "senha123";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var tipo = eTipoUsuario.Medico;

        // Act
        var usuario = new Usuario(nome, email, senhaHash, tipo);

        // Assert
        Assert.Equal(nome, usuario.Nome);
        Assert.Equal(email, usuario.Email);
        Assert.Equal(senhaHash, usuario.SenhaHash);
        Assert.Equal(tipo, usuario.TipoUsuario);
        Assert.True(usuario.Ativo);
    }

    [Fact]
    public void DefaultConstructor_SetsPropertiesToDefaults()
    {
        // Act
        var usuario = new Usuario();

        // Assert
        Assert.Null(usuario.Nome);
        Assert.Null(usuario.Email);
        Assert.Null(usuario.SenhaHash);
        Assert.Equal(default(eTipoUsuario), usuario.TipoUsuario);
        Assert.True(usuario.Ativo);
    }

    [Fact]
    public void ValidarSenha_ReturnsTrue_ForCorrectPassword()
    {
        // Arrange
        var senha = "senha123";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario("Fulano", "fulano@teste.com", senhaHash, eTipoUsuario.Medico);

        // Act
        var isValid = usuario.ValidarSenha(senha);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidarSenha_ReturnsFalse_ForIncorrectPassword()
    {
        // Arrange
        var senha = "senha123";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario("Fulano", "fulano@teste.com", senhaHash, eTipoUsuario.Medico);

        // Act
        var isValid = usuario.ValidarSenha("senhaErrada");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void AlterarSenha_ChangesHash_AndValidatesNewPassword()
    {
        // Arrange
        var senhaInicial = "senha123";
        var senhaNova = "novaSenha456";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaInicial);
        var usuario = new Usuario("Fulano", "fulano@teste.com", senhaHash, eTipoUsuario.Medico);

        // Act
        usuario.AlterarSenha(senhaNova);

        // Assert
        Assert.NotEqual(senhaHash, usuario.SenhaHash);
        Assert.True(usuario.ValidarSenha(senhaNova));
        Assert.False(usuario.ValidarSenha(senhaInicial));
    }

    [Fact]
    public void DesativarUsuario_SetsAtivoToFalse()
    {
        // Arrange
        var senha = "senha123";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario("Fulano", "fulano@teste.com", senhaHash, eTipoUsuario.Medico);

        // Act
        usuario.DesativarUsuario();

        // Assert
        Assert.False(usuario.Ativo);
    }
}
