using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Repository;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Infra;

[Collection(nameof(ContextDbCollection))]
[Order(7)]
public class MedicoRepositoryTest
{
    private readonly AppDBContext _context;
    private readonly IMedicoRepository _repository;
    private readonly ContextDbFixture _fixture;


    public MedicoRepositoryTest(ContextDbFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context!;
        _repository = new MedicoRepository(_context);
    }


    [Fact, Order(1)]
    public async Task BuscarMedicoPorUsuarioId_DeveRetornarIdMedico_QuandoEncontrado()
    {
        await _fixture.ResetDatabaseAsync();

        // Arrange: Cria um usuário do tipo Médico e registra o médico
        var usuario = new Usuario("Medico Exemplo", "medico@example.com", "hashSenha", eTipoUsuario.Medico);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var medico = new Medico(usuario.Id, "CRM12345", eEspecialidade.Cardiologia);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        // Act: Recupera o médico pelo ID do usuário
        var resultado = await _repository.BuscarMedicoPorUsuarioId(usuario.Id);

        // Assert: Deve retornar o ID do médico cadastrado
        Assert.NotEqual(Guid.Empty, resultado);
        Assert.Equal(medico.Id, resultado);
    }



    [Fact, Order(2)]
    public async Task BuscarMedicoPorUsuarioId_DeveRetornarGuidEmpty_QuandoMedicoNaoEncontrado()
    {
        await _fixture.ResetDatabaseAsync();

        // Act: Passa um Guid aleatório (sem médico cadastrado)
        var resultado = await _repository.BuscarMedicoPorUsuarioId(Guid.NewGuid());

        // Assert: Como não existe médico para esse usuário, retorna Guid.Empty
        Assert.Equal(Guid.Empty, resultado);
    }

    [Fact, Order(3)]
    public async Task BuscarMedicos_DeveRetornarMedicos_CorretamenteFiltrados()
    {
        await _fixture.ResetDatabaseAsync();

        // Arrange: Limpa e semeia dados (ou adicione medicos sem se preocupar com duplicação se o fixture já limpar)
        // Cria dois usuários do tipo Médico
        var usuario1 = new Usuario("Medico 1", "medico1@example.com", "hashSenha", eTipoUsuario.Medico);
        var usuario2 = new Usuario("Medico 2", "medico2@example.com", "hashSenha", eTipoUsuario.Medico);
        _context.Usuarios.AddRange(usuario1, usuario2);
        await _context.SaveChangesAsync();

        // Cria os registros de médicos com especialidades distintas
        var medico1 = new Medico(usuario1.Id, "CRM11111", eEspecialidade.Cardiologia);
        var medico2 = new Medico(usuario2.Id, "CRM22222", eEspecialidade.OrtopediaTraumatologia);
        _context.Medicos.AddRange(medico1, medico2);
        await _context.SaveChangesAsync();

        // Act: Busca todos os médicos (sem filtro)
        var todosMedicos = await _repository.BuscarMedicos(null);
        // Act: Busca apenas os de Cardiologia
        var medicosCardiologia = await _repository.BuscarMedicos(eEspecialidade.Cardiologia);

        // Assert: Verifica se ao buscar sem filtro, pelo menos os dois estão presentes
        Assert.NotNull(todosMedicos);
        Assert.True(todosMedicos.Count() >= 2);

        // E que ao filtrar por Cardiologia, todos os resultados possuam essa especialidade
        Assert.NotNull(medicosCardiologia);
        Assert.All(medicosCardiologia, m => Assert.Equal(eEspecialidade.Cardiologia, m.Especialidade));
    }
}
