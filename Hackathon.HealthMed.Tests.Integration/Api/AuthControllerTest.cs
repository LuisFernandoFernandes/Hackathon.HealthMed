using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Api
{
    [Order(1)]
    public class AuthControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>, IClassFixture<ContextDbFixture>
    {
        private readonly HttpClient _client;
        private readonly AppDBContext _context;

        public AuthControllerTest(CustomWebApplicationFactory<Program> factory, ContextDbFixture contextDbFixture)
        {
            // Configura a connection string para o container do SQL Server
            factory.conectionString = contextDbFixture.sqlConnection;
            _context = contextDbFixture.Context!;

            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = true
            });
        }

        [Fact, Order(1)]
        public async Task AuthController_Login_DeveRetornarTokenParaMedico()
        {
            // Arrange
            // Limpa os dados das tabelas para evitar conflitos com FK
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Agendamentos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Horarios");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Medicos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Pacientes");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Usuarios");

            // Semente: Inserir um usuário do tipo Médico
            string senhaOriginal = "senha123";
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com", BCrypt.Net.BCrypt.HashPassword(senhaOriginal), eTipoUsuario.Medico);
            _context.Usuarios.Add(usuarioMedico);
            _context.SaveChanges();

            // Insere o médico com CRM, que será usado como login
            var medico = new Medico(usuarioMedico.Id, "CRM123456", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            _context.SaveChanges();

            // Cria o payload de login para o médico
            var loginRequest = new LoginRequestDTO
            {
                Login = "CRM123456", // para médicos, o login é o CRM
                Senha = senhaOriginal,
                TipoUsuario = eTipoUsuario.Medico
            };

            // Act
            var response = await _client.PostAsJsonAsync("/login", loginRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            Assert.NotNull(loginResponse);
            Assert.False(string.IsNullOrEmpty(loginResponse.Token));
        }

        [Fact, Order(2)]
        public async Task AuthController_Login_DeveRetornarNaoAutorizadoParaCredenciaisInvalidas()
        {
            // Arrange
            // Limpa os dados das tabelas
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Agendamentos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Horarios");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Medicos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Pacientes");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Usuarios");

            // Semente: Inserir um usuário do tipo Paciente
            string senhaOriginal = "senha456";
            var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com", BCrypt.Net.BCrypt.HashPassword(senhaOriginal), eTipoUsuario.Paciente);
            _context.Usuarios.Add(usuarioPaciente);
            _context.SaveChanges();

            // Insere o paciente com CPF (ou utilize o email para login)
            var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
            _context.Pacientes.Add(paciente);
            _context.SaveChanges();

            // Cria o payload de login com senha incorreta
            var loginRequest = new LoginRequestDTO
            {
                Login = "paciente@exemplo.com",
                Senha = "senhaErrada",
                TipoUsuario = eTipoUsuario.Paciente
            };

            // Act
            var response = await _client.PostAsJsonAsync("/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Usuário ou senha inválidos", responseString);
        }
    }
}
