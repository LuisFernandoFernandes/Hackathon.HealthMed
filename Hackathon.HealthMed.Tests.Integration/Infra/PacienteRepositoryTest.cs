using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Repository;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Infra;

[Collection(nameof(ContextDbCollection))]
[Order(3)]
public class PacienteRepositoryTest
{
    private readonly AppDBContext _context;
    private readonly IPacienteRepository _repository;

    public PacienteRepositoryTest(ContextDbFixture fixture)
    {
        _context = fixture.Context!;
        _repository = new PacienteRepository(_context);
    }

    [Fact]
    public async Task BuscarPacientePorUsuarioId_DeveRetornarIdPacienteQuandoEncontrado()
    {
        // Arrange: Cria um usuário do tipo Paciente e o registro de paciente associado
        var usuario = new Usuario("Paciente Teste", "paciente@exemplo.com", "hashSenha", eTipoUsuario.Paciente);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var paciente = new Paciente(usuario.Id, "12345678900");
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        // Act: Chama o método passando o Id do usuário
        var result = await _repository.BuscarPacientePorUsuarioId(usuario.Id);

        // Assert: O Id retornado deve ser o do paciente criado
        Assert.NotEqual(Guid.Empty, result);
        Assert.Equal(paciente.Id, result);
    }

    [Fact]
    public async Task BuscarPacientePorUsuarioId_DeveRetornarGuidEmptyQuandoNaoEncontrado()
    {
        // Arrange: Gera um Guid aleatório que não tenha paciente associado
        var randomUserId = Guid.NewGuid();

        // Act
        var result = await _repository.BuscarPacientePorUsuarioId(randomUserId);

        // Assert: Como não há paciente para o usuário, o resultado deve ser Guid.Empty
        Assert.Equal(Guid.Empty, result);
    }
}
