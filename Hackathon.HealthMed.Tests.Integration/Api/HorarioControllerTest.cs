using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Api
{
    [Order(3)]
    public class HorarioControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>, IClassFixture<ContextDbFixture>
    {
        private readonly HttpClient _client;
        private readonly AppDBContext _context;

        public HorarioControllerTest(CustomWebApplicationFactory<Program> factory, ContextDbFixture contextDbFixture)
        {
            factory.conectionString = contextDbFixture.sqlConnection;
            _context = contextDbFixture.Context!;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });
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
        public async Task HorarioController_CadastrarHorario_DeveCadastrarHorario_ComSucesso()
        {
            // Arrange: limpar tabelas e semear usuário e médico
            ClearDatabase();

            string senha = "senhaMedico";
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
                BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
            _context.Usuarios.Add(usuarioMedico);
            _context.SaveChanges();

            var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            _context.SaveChanges();

            // Configura o cliente para simular um usuário com role "Medico"
            _client.DefaultRequestHeaders.Remove("X-Test-UserId");
            _client.DefaultRequestHeaders.Remove("X-Test-Roles");
            _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioMedico.Id.ToString());
            _client.DefaultRequestHeaders.Add("X-Test-Roles", "Medico");

            // Monta o DTO para cadastrar um horário, sem informar MedicoId (será obtido via claim)
            var cadastrarHorarioDTO = new CadastrarHorarioDTO
            {
                DataHorario = DateTime.UtcNow.AddHours(1),
                Valor = 100
            };

            // Act: chama o endpoint POST /api/horario
            var response = await _client.PostAsJsonAsync("/api/horario", cadastrarHorarioDTO);

            // Assert: espera status OK e um Guid válido como resultado
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var resultado = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, resultado);
        }

        [Fact, Order(2)]
        public async Task HorarioController_EditarHorario_DeveEditarHorario_ComSucesso()
        {
            // Arrange: limpar e semear dados
            ClearDatabase();

            string senha = "senhaMedico";
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
                BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
            _context.Usuarios.Add(usuarioMedico);
            _context.SaveChanges();

            var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            _context.SaveChanges();

            // Criar um horário existente para edição
            var horario = new Horario(medico.Id, DateTime.UtcNow.AddHours(2), 150);
            _context.Horarios.Add(horario);
            _context.SaveChanges();

            // Configura o cliente para simular um usuário com role "Medico"
            _client.DefaultRequestHeaders.Remove("X-Test-UserId");
            _client.DefaultRequestHeaders.Remove("X-Test-Roles");
            _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioMedico.Id.ToString());
            _client.DefaultRequestHeaders.Add("X-Test-Roles", "Medico");

            // Cria o DTO para editar o horário
            var editarHorarioDTO = new EditarHorarioDTO
            {
                Id = horario.Id,
                DataHorario = horario.DataHorario.AddHours(1),
                Valor = 200
            };

            // Act: chama o endpoint PUT /api/horario
            var response = await _client.PutAsJsonAsync("/api/horario", editarHorarioDTO);

            // Assert: espera status OK e que a resposta seja um booleano true
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var resultado = await response.Content.ReadFromJsonAsync<bool>();
            Assert.True(resultado);
        }

        [Fact, Order(3)]
        public async Task HorarioController_BuscarMeusHorarios_DeveRetornarHorarios_DoMedicoAutenticado()
        {
            // Arrange: limpar e semear dados
            ClearDatabase();

            string senha = "senhaMedico";
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
                BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
            _context.Usuarios.Add(usuarioMedico);
            _context.SaveChanges();

            var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            _context.SaveChanges();

            // Criar alguns horários para o médico autenticado
            var horario1 = new Horario(medico.Id, DateTime.UtcNow.AddHours(1), 100);
            var horario2 = new Horario(medico.Id, DateTime.UtcNow.AddHours(2), 150);
            _context.Horarios.AddRange(horario1, horario2);
            _context.SaveChanges();

            // Configura o cliente para simular um usuário com role "Medico"
            _client.DefaultRequestHeaders.Remove("X-Test-UserId");
            _client.DefaultRequestHeaders.Remove("X-Test-Roles");
            _client.DefaultRequestHeaders.Add("X-Test-UserId", usuarioMedico.Id.ToString());
            _client.DefaultRequestHeaders.Add("X-Test-Roles", "Medico");

            // Act: chama o endpoint GET /api/horario
            var response = await _client.GetAsync("/api/horario");

            // Assert: espera status OK e que a lista retornada contenha os horários criados
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var resultado = await response.Content.ReadFromJsonAsync<IEnumerable<HorarioDTO>>();
            Assert.NotNull(resultado);
            Assert.Contains(resultado, h => h.Id == horario1.Id);
            Assert.Contains(resultado, h => h.Id == horario2.Id);
        }

        [Fact, Order(4)]
        public async Task HorarioController_BuscarHorariosPorMedico_DeveRetornarHorarios_Disponiveis()
        {
            // Arrange: limpar e semear dados
            ClearDatabase();

            string senha = "senhaMedico";
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com",
                BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
            _context.Usuarios.Add(usuarioMedico);
            _context.SaveChanges();

            var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            _context.SaveChanges();

            var horario1 = new Horario(medico.Id, DateTime.UtcNow.AddHours(3), 120);
            var horario2 = new Horario(medico.Id, DateTime.UtcNow.AddHours(4), 130);
            _context.Horarios.AddRange(horario1, horario2);
            _context.SaveChanges();

            // Configura o cliente para simular um usuário com role "Paciente"
            _client.DefaultRequestHeaders.Remove("X-Test-UserId");
            _client.DefaultRequestHeaders.Remove("X-Test-Roles");
            _client.DefaultRequestHeaders.Add("X-Test-UserId", Guid.NewGuid().ToString());
            _client.DefaultRequestHeaders.Add("X-Test-Roles", "Paciente");

            // Act: chama o endpoint GET /api/horario/medico/{medicoId}
            var response = await _client.GetAsync($"/api/horario/medico/{medico.Id}");

            // Assert: espera status OK e que a lista retornada contenha os horários criados
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var resultado = await response.Content.ReadFromJsonAsync<IEnumerable<HorarioDTO>>();
            Assert.NotNull(resultado);
            Assert.Contains(resultado, h => h.Id == horario1.Id);
            Assert.Contains(resultado, h => h.Id == horario2.Id);
        }
    }
}
