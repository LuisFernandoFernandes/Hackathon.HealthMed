using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Api;

[Order(4)]
public class AgendamentoControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>, IClassFixture<ContextDbFixture>
{
    private readonly HttpClient _client;
    private readonly AppDBContext _context;

    public AgendamentoControllerTest(CustomWebApplicationFactory<Program> factory, ContextDbFixture contextDbFixture)
    {
        factory.conectionString = contextDbFixture.sqlConnection;
        _context = contextDbFixture.Context!;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
    }

    private void ClearDatabase()
    {
        _context.Database.ExecuteSqlRaw("DELETE FROM Agendamentos");
        _context.Database.ExecuteSqlRaw("DELETE FROM Horarios");
        _context.Database.ExecuteSqlRaw("DELETE FROM Medicos");
        _context.Database.ExecuteSqlRaw("DELETE FROM Pacientes");
        _context.Database.ExecuteSqlRaw("DELETE FROM Usuarios");
    }

    [Fact, Order(1)]
    public async Task AgendamentoController_AgendarConsulta_DeveAgendarConsulta_ComSucesso()
    {
        // Arrange
        ClearDatabase();

        // Semear um usuário do tipo Paciente e criar seu registro no Paciente
        string senhaPaciente = "senhaPaciente";
        var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaPaciente), eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuarioPaciente);
        _context.SaveChanges();

        // Criar registro de Paciente
        var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
        _context.Pacientes.Add(paciente);
        _context.SaveChanges();

        // Semear um médico (necessário para o horário)
        string senhaMedico = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaMedico), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();

        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Criar um horário disponível para agendamento
        var horario = new Horario(medico.Id, DateTime.UtcNow.AddHours(2), 150);
        _context.Horarios.Add(horario);
        _context.SaveChanges();

        // Configura o cliente para simular um usuário do tipo Paciente
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-Roles");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioPaciente.Id.ToString());
        _client.DefaultRequestHeaders.Add("X-Test-Roles", "Paciente");

        // Monta o DTO para agendar a consulta (apenas HorarioId e, se houver, outros campos obrigatórios)
        var agendarConsultaDTO = new AgendarConsultaDTO
        {
            HorarioId = horario.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/agendamento", agendarConsultaDTO);

        // Assert: espera status OK e um Guid válido (ID do agendamento)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, resultado);
    }

    [Fact, Order(2)]
    public async Task AgendamentoController_ConfirmarAgendamento_DeveConfirmarConsulta_ComSucesso()
    {
        // Arrange
        ClearDatabase();

        // Semear um usuário do tipo Paciente e seu registro
        string senhaPaciente = "senhaPaciente";
        var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaPaciente), eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuarioPaciente);
        _context.SaveChanges();
        var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
        _context.Pacientes.Add(paciente);
        _context.SaveChanges();

        // Semear um usuário do tipo Medico e seu registro
        string senhaMedico = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaMedico), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();
        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Criar um horário reservado (status Reservado) vinculado ao médico
        var horario = new Horario(medico.Id, DateTime.UtcNow.AddHours(3), 150);
        horario.AtualizarStatus(eStatusHorario.Reservado);
        _context.Horarios.Add(horario);
        _context.SaveChanges();

        // Criar um agendamento vinculado ao paciente e ao horário
        var agendamento = new Agendamento(paciente.Id, horario.Id);
        _context.Agendamentos.Add(agendamento);
        _context.SaveChanges();

        // Configura o cliente para simular um usuário com role "Medico"
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-Roles");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioMedico.Id.ToString());
        _client.DefaultRequestHeaders.Add("X-Test-Roles", "Medico");

        // Monta o DTO para confirmar o agendamento (Aceitar = true)
        var confirmarAgendamentoDTO = new ConfirmarAgendamentoDTO
        {
            AgendamentoId = agendamento.Id,
            Aceitar = true
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/agendamento/confirmar", confirmarAgendamentoDTO);

        // Assert: espera status OK e um booleano true
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<bool>();
        Assert.True(resultado);
    }

    [Fact, Order(3)]
    public async Task AgendamentoController_CancelarConsultaPaciente_DeveCancelarConsulta_ComSucesso()
    {
        // Arrange
        ClearDatabase();

        // Semear um usuário do tipo Paciente e seu registro
        string senhaPaciente = "senhaPaciente";
        var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaPaciente), eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuarioPaciente);
        _context.SaveChanges();
        var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
        _context.Pacientes.Add(paciente);
        _context.SaveChanges();

        // Semear um usuário do tipo Medico e seu registro (para criar o horário)
        string senhaMedico = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaMedico), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();
        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Criar um horário reservado e um agendamento (status Agendado ou Pendente)
        var horario = new Horario(medico.Id, DateTime.UtcNow.AddHours(4), 150);
        horario.AtualizarStatus(eStatusHorario.Reservado);
        _context.Horarios.Add(horario);
        _context.SaveChanges();

        var agendamento = new Agendamento(paciente.Id, horario.Id);
        // Para simular um agendamento ativo, garanta que o status seja Agendado (ou Pendente)
        agendamento.AtualizarStatus(eStatusAgendamento.Agendado);
        _context.Agendamentos.Add(agendamento);
        _context.SaveChanges();

        // Configura o cliente para simular um usuário com role "Paciente"
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-Roles");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioPaciente.Id.ToString());
        _client.DefaultRequestHeaders.Add("X-Test-Roles", "Paciente");

        // Monta o DTO para cancelar a consulta pelo paciente
        var cancelarPacienteDTO = new CancelarAgendamentoPacienteDTO
        {
            AgendamentoId = agendamento.Id,
            Justificativa = "Não posso comparecer."
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/agendamento/cancelar-paciente", cancelarPacienteDTO);

        // Assert: espera status OK e um booleano true
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<bool>();
        Assert.True(resultado);
    }

    [Fact, Order(4)]
    public async Task AgendamentoController_CancelarConsultaMedico_DeveCancelarConsulta_ComSucesso()
    {
        // Arrange
        ClearDatabase();

        // Semear um usuário do tipo Paciente e seu registro
        string senhaPaciente = "senhaPaciente";
        var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaPaciente), eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuarioPaciente);
        _context.SaveChanges();
        var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
        _context.Pacientes.Add(paciente);
        _context.SaveChanges();

        // Semear um usuário do tipo Medico e seu registro
        string senhaMedico = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
            BCrypt.Net.BCrypt.HashPassword(senhaMedico), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();
        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Criar um horário reservado e um agendamento para cancelamento
        var horario = new Horario(medico.Id, DateTime.UtcNow.AddHours(5), 150);
        horario.AtualizarStatus(eStatusHorario.Reservado);
        _context.Horarios.Add(horario);
        _context.SaveChanges();

        var agendamento = new Agendamento(paciente.Id, horario.Id);
        agendamento.AtualizarStatus(eStatusAgendamento.Agendado);
        _context.Agendamentos.Add(agendamento);
        _context.SaveChanges();

        // Configura o cliente para simular um usuário com role "Medico"
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-Roles");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioMedico.Id.ToString());
        _client.DefaultRequestHeaders.Add("X-Test-Roles", "Medico");

        // Monta o DTO para cancelar a consulta pelo médico
        var cancelarMedicoDTO = new CancelarAgendamentoMedicoDTO
        {
            AgendamentoId = agendamento.Id,
            Justificativa = "Consulta não poderá ser realizada."
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/agendamento/cancelar-medico", cancelarMedicoDTO);

        // Assert: espera status OK e um booleano true
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<bool>();
        Assert.True(resultado);
    }
}
