using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Tests.Unit.Domain.Entities;

public class MedicoTest
{
    [Fact]
    public void Constructor_ComParametros_DeveDefinirPropriedadesCorretamente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var crm = "123456";
        var especialidade = eEspecialidade.Cardiologia;

        // Act
        var medico = new Medico(usuarioId, crm, especialidade);

        // Assert
        Assert.Equal(usuarioId, medico.UsuarioId);
        Assert.Equal(crm, medico.CRM);
        Assert.Equal(especialidade, medico.Especialidade);
    }

    [Fact]
    public void Constructor_Default_DeveTerValoresPadrao()
    {
        // Arrange & Act
        var medico = new Medico();

        // Assert
        Assert.Equal(Guid.Empty, medico.UsuarioId);
        Assert.Null(medico.CRM);
        Assert.Equal(default(eEspecialidade), medico.Especialidade);
        Assert.Null(medico.Usuario);
    }

    [Fact]
    public void Usuario_Property_PodeSerModificada()
    {
        // Arrange
        var medico = new Medico();
        var usuario = new Usuario("Nome Teste", "teste@teste.com", "hashDeSenha", eTipoUsuario.Medico);

        // Act
        medico.Usuario = usuario;

        // Assert
        Assert.Equal(usuario, medico.Usuario);
    }
}
