using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Tests.Unit.Domain.Entities;

public class AgendamentoTest
{
    [Fact]
    public void Constructor_ComParametros_DeveDefinirPropriedadesEStatusPadrao()
    {
        // Arrange
        var pacienteId = Guid.NewGuid();
        var horarioId = Guid.NewGuid();

        // Act
        var agendamento = new Agendamento(pacienteId, horarioId);

        // Assert
        Assert.Equal(pacienteId, agendamento.PacienteId);
        Assert.Equal(horarioId, agendamento.HorarioId);
        Assert.Equal(eStatusAgendamento.Pendente, agendamento.Status);
        Assert.Null(agendamento.JustificativaCancelamento);
    }

    [Fact]
    public void AtualizarStatus_SemJustificativa_DeveAtualizarStatusSemJustificativa()
    {
        // Arrange
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid());

        // Act
        agendamento.AtualizarStatus(eStatusAgendamento.Agendado);

        // Assert
        Assert.Equal(eStatusAgendamento.Agendado, agendamento.Status);
        Assert.Null(agendamento.JustificativaCancelamento);
    }

    [Fact]
    public void AtualizarStatus_ComJustificativa_DeveAtualizarStatusEJustificativa()
    {
        // Arrange
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid());
        var justificativa = "Motivo de cancelamento";

        // Act
        agendamento.AtualizarStatus(eStatusAgendamento.Cancelado, justificativa);

        // Assert
        Assert.Equal(eStatusAgendamento.Cancelado, agendamento.Status);
        Assert.Equal(justificativa, agendamento.JustificativaCancelamento);
    }
}
