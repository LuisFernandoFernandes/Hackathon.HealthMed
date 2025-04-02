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

[Order(2)]
public class MedicoControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>, IClassFixture<ContextDbFixture>
{
    private readonly HttpClient _client;
    private readonly AppDBContext _context;
    private readonly ContextDbFixture _fixture;

    public MedicoControllerTest(CustomWebApplicationFactory<Program> factory, ContextDbFixture contextDbFixture)
    {
        _fixture = contextDbFixture;
        // Configura a connection string para o container SQL
        factory.conectionString = contextDbFixture.sqlConnection;
        _context = contextDbFixture.Context!;

        // Cria o client para os testes – lembre-se de que a configuração de autenticação
        // deve garantir que a requisição seja tratada como um usuário com role "Paciente".
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
    }

    [Fact, Order(1)]
    public async Task MedicoController_GetMedicos_DeveRetornarListaDeMedicos()
    {
        // Arrange: Limpar as tabelas e semear dados
        await _fixture.ResetDatabaseAsync();

        // Semente: Inserir um usuário do tipo Médico e o respectivo registro de Médico
        string senha = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com", BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();

        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Obs.: Para que o endpoint seja acessado, a requisição precisa ser autenticada com um usuário do tipo Paciente.

        // Act: Requisição GET para o endpoint, filtrando por especialidade "Cardiologia"
        var response = await _client.GetAsync("/api/medico?especialidade=Cardiologia");

        // Assert: Verifica se a resposta é 200 OK e se o médico inserido é retornado
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var medicosDto = await response.Content.ReadFromJsonAsync<IEnumerable<MedicoDTO>>();
        Assert.NotNull(medicosDto);
        Assert.Contains(medicosDto, m => m.CRM == "CRM0001");
    }

    [Fact, Order(2)]
    public async Task MedicoController_GetMedicos_DeveRetornarForbiddenParaUsuarioMedico()
    {
        // Arrange: Limpar as tabelas e semear dados
        await _fixture.ResetDatabaseAsync();

        // Semente: Inserir um usuário do tipo Médico e o respectivo registro de Médico
        string senha = "senhaMedico";
        var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com", BCrypt.Net.BCrypt.HashPassword(senha), eTipoUsuario.Medico);
        _context.Usuarios.Add(usuarioMedico);
        _context.SaveChanges();

        var medico = new Medico(usuarioMedico.Id, "CRM0001", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        _context.SaveChanges();

        // Act: Cria uma requisição GET para o endpoint com o cabeçalho "X-Test-Roles" definido como "Medico"
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/medico?especialidade=Cardiologia");
        request.Headers.Add("X-Test-Roles", "Medico");
        var response = await _client.SendAsync(request);

        // Assert: Como o endpoint exige role "Paciente", a resposta deve ser Forbidden (403)
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}