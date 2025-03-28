using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Tests.Unit.Domain.Entities;

public class HorarioTest
{
    [Fact]
    public void Constructor_ComParametros_DeveDefinirPropriedadesEStatusPadrao()
    {
        // Arrange
        var medicoId = Guid.NewGuid();
        var dataHorario = DateTime.UtcNow.AddDays(1);
        var valor = 100.50M;

        // Act
        var horario = new Horario(medicoId, dataHorario, valor);

        // Assert
        Assert.Equal(medicoId, horario.MedicoId);
        Assert.Equal(dataHorario, horario.DataHorario);
        Assert.Equal(valor, horario.Valor);
        Assert.Equal(eStatusHorario.Disponivel, horario.Status);
    }

    [Fact]
    public void AtualizarStatus_DeveAtualizarStatusCorretamente()
    {
        // Arrange
        var medicoId = Guid.NewGuid();
        var dataHorario = DateTime.UtcNow.AddDays(1);
        var valor = 200.75M;
        var horario = new Horario(medicoId, dataHorario, valor);

        // Act
        horario.AtualizarStatus(eStatusHorario.Reservado);

        // Assert
        Assert.Equal(eStatusHorario.Reservado, horario.Status);
    }

    [Fact]
    public void Constructor_Default_DeveTerValoresPadrao()
    {
        // Arrange & Act
        var horario = new Horario();

        // Assert
        Assert.Equal(Guid.Empty, horario.MedicoId);
        Assert.Equal(default(DateTime), horario.DataHorario);
        Assert.Equal(0, horario.Valor);
        Assert.Equal(eStatusHorario.Disponivel, horario.Status);
    }
}
