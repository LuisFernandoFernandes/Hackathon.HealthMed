using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Tests.Unit.Domain.Entities;

public class PacienteTest
{
    [Fact]
    public void Constructor_WithParameters_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var cpf = "12345678901";

        // Act
        var paciente = new Paciente(usuarioId, cpf);

        // Assert
        Assert.Equal(usuarioId, paciente.UsuarioId);
        Assert.Equal(cpf, paciente.Cpf);
        Assert.Null(paciente.Usuario);
    }

    [Fact]
    public void Constructor_Default_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var paciente = new Paciente();

        // Assert
        Assert.Equal(Guid.Empty, paciente.UsuarioId);
        Assert.Null(paciente.Cpf);
        Assert.Null(paciente.Usuario);
    }
}
