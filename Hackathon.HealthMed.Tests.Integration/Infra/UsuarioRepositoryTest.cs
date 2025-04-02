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
[Order(5)]
public class UsuarioRepositoryTest
{
    private readonly AppDBContext _context;
    private readonly IUsuarioRepository _repository;
    private readonly ContextDbFixture _fixture;

    public UsuarioRepositoryTest(ContextDbFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context!;
        _repository = new UsuarioRepository(_context);
    }


    [Fact, Order(1)]
    public async Task ObterPorLogin_Medico_DeveRetornarUsuario()
    {
        await _fixture.ResetDatabaseAsync();
        // Arrange: Cria um usuário e um registro de médico associado com um CRM específico.
        var usuario = new Usuario("Medico Exemplo", "medico@example.com", "hashSenha", eTipoUsuario.Medico);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var medico = new Medico(usuario.Id, "CRM12345", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        // Act: Recupera o usuário a partir do CRM (login) para o tipo Medico.
        var resultado = await _repository.ObterPorLogin("CRM12345", eTipoUsuario.Medico);

        // Assert: Verifica se o usuário retornado não é nulo e se é o mesmo que foi inserido.
        Assert.NotNull(resultado);
        Assert.Equal(usuario.Id, resultado.Id);
    }

    [Fact, Order(2)]
    public async Task ObterPorLogin_PacienteComEmail_DeveRetornarUsuario()
    {
        await _fixture.ResetDatabaseAsync();
        // Arrange: Cria um usuário do tipo Paciente com e-mail.
        var usuario = new Usuario("Paciente Exemplo", "paciente@example.com", "hashSenha", eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Act: Tenta obter o usuário passando o e-mail (contendo '@') como login.
        var resultado = await _repository.ObterPorLogin("paciente@example.com", eTipoUsuario.Paciente);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(usuario.Id, resultado.Id);
    }

    [Fact, Order(3)]
    public async Task ObterPorLogin_PacienteComCPF_DeveRetornarUsuario()
    {
        await _fixture.ResetDatabaseAsync();
        // Arrange: Cria um usuário do tipo Paciente e um registro de paciente associado com um CPF.
        var usuario = new Usuario("Paciente CPF", "outraemail@example.com", "hashSenha", eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var paciente = new Paciente(usuario.Id, "12345678900");
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        // Act: Recupera o usuário passando o CPF (sem '@') como login.
        var resultado = await _repository.ObterPorLogin("12345678900", eTipoUsuario.Paciente);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(usuario.Id, resultado.Id);
    }

    [Fact, Order(4)]
    public async Task ObterPorLogin_LoginInexistente_DeveRetornarNull()
    {
        // Arrange: Limpa o banco antes do teste
        await _fixture.ResetDatabaseAsync();

        // Act: Tenta obter um usuário com um login que não existe.
        var resultado = await _repository.ObterPorLogin("inexistente", eTipoUsuario.Paciente);

        // Assert
        Assert.Null(resultado);
    }
}
