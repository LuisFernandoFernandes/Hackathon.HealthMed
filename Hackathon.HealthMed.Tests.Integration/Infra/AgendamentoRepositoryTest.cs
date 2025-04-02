using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Repository;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.EntityFrameworkCore;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Infra;

[Collection(nameof(ContextDbCollection))]
[Order(9)]
public class AgendamentoRepositoryTest
{
    private readonly AppDBContext _context;
    private readonly IAgendamentoRepository _repository;
    private readonly ContextDbFixture _fixture;

    public AgendamentoRepositoryTest(ContextDbFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context!;
        _repository = new AgendamentoRepository(_context);
    }

    /// <summary>
    /// Semeia os dados necessários para cadastrar um agendamento: usuário e registro de paciente; 
    /// além de um usuário/médico e um horário.
    /// Retorna uma tupla (pacienteId, horarioId).
    /// </summary>
    private async Task<(Guid pacienteId, Guid horarioId)> SeedAgendamentoDependenciesAsync()
    {
        await _fixture.ResetDatabaseAsync();
        // Cria um usuário do tipo Paciente
        var usuarioPaciente = new Usuario("Paciente Teste", "paciente@example.com", "hashSenha", eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuarioPaciente);
        await _context.SaveChangesAsync();

        // Cria o registro de Paciente
        var paciente = new Paciente(usuarioPaciente.Id, "12345678900");
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        // Cria um usuário do tipo Médico
        var usuarioMedico = new Usuario("Medico Teste", "medico@example.com", "hashSenha", eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        await _context.SaveChangesAsync();

        // Cria o registro de Médico
        var medico = new Medico(usuarioMedico.Id, "CRM12345", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        // Cria um horário disponível para agendamento
        var dataHorario = DateTime.UtcNow.AddDays(1);
        var horario = new Horario(medico.Id, dataHorario, 150m);
        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();

        return (paciente.Id, horario.Id);
    }

    [Fact, Order(1)]
    public async Task Adicionar_DeveInserirAgendamentoNoBanco()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        var (pacienteId, horarioId) = await SeedAgendamentoDependenciesAsync();

        var agendamento = new Agendamento(pacienteId, horarioId);

        // Act
        await _repository.Adicionar(agendamento);

        // Assert
        var inserted = await _context.Agendamentos.FindAsync(agendamento.Id);
        Assert.NotNull(inserted);
        Assert.Equal(pacienteId, inserted.PacienteId);
        Assert.Equal(horarioId, inserted.HorarioId);
        // O status padrão deve ser Pendente, conforme a definição da entidade
        Assert.Equal(eStatusAgendamento.Pendente, inserted.Status);
    }

    [Fact, Order(2)]

    public async Task Editar_DeveAtualizarAgendamentoNoBanco()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        var (pacienteId, horarioId) = await SeedAgendamentoDependenciesAsync();

        var agendamento = new Agendamento(pacienteId, horarioId);
        await _repository.Adicionar(agendamento);

        // Simule uma alteração no status: vamos alterar para Agendado.
        // Caso o setter de Status não seja público, usamos reflection:
        var statusProp = typeof(Agendamento)
            .GetProperty("Status", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        statusProp.SetValue(agendamento, eStatusAgendamento.Agendado);

        // Act
        await _repository.Editar(agendamento);

        // Assert
        var updated = await _context.Agendamentos.FindAsync(agendamento.Id);
        Assert.NotNull(updated);
        Assert.Equal(eStatusAgendamento.Agendado, updated.Status);
    }

    [Fact, Order(3)]

    public async Task BuscarPorId_DeveRetornarAgendamento_QuandoExistir()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        var (pacienteId, horarioId) = await SeedAgendamentoDependenciesAsync();

        var agendamento = new Agendamento(pacienteId, horarioId);
        await _repository.Adicionar(agendamento);

        // Act
        var result = await _repository.BuscarPorId(agendamento.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(agendamento.Id, result.Id);
    }

    [Fact, Order(4)]

    public async Task BuscarPorId_DeveRetornarNull_QuandoNaoExistir()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        // Act
        var result = await _repository.BuscarPorId(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact, Order(5)]

    public async Task ConsultarAgendamentosPorUsuario_DeveRetornarAgendamentosDoDiaAtual()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        var (pacienteId, horarioId) = await SeedAgendamentoDependenciesAsync();

        // Cria um horário para hoje (ou futuro) e um agendamento associado
        var dataHorario = DateTime.Today.AddHours(14);
        // Atualiza o horário do agendamento sem alterar as dependências já criadas

        var agendamento = new Agendamento(pacienteId, horarioId);
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ConsultarAgendamentosPorUsuario(eTipoUsuario.Paciente, pacienteId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
    }

}
