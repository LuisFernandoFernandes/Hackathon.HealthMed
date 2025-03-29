using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Services;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Moq;

namespace Hackathon.HealthMed.Tests.Unit.Application.Services;

public class MedicoServiceTest
{
    private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MedicoService _medicoService;

    public MedicoServiceTest()
    {
        _medicoRepositoryMock = new Mock<IMedicoRepository>();
        _mapperMock = new Mock<IMapper>();
        _medicoService = new MedicoService(_medicoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task BuscarMedicos_DeveRetornarMedicosDTO_QuandoRepositorioRetornarMedicos()
    {
        // Arrange
        eEspecialidade? especialidade = eEspecialidade.Cardiologia;
        var medicos = new List<Medico>
        {
            new Medico(Guid.NewGuid(), "CRM123", eEspecialidade.Cardiologia)
        };
        var medicosDto = new List<MedicoDTO>
        {
            new MedicoDTO { CRM = "CRM123", Especialidade = eEspecialidade.Cardiologia }
        };

        _medicoRepositoryMock
            .Setup(r => r.BuscarMedicos(especialidade))
            .ReturnsAsync(medicos);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<Medico>, IEnumerable<MedicoDTO>>(medicos))
            .Returns(medicosDto);

        // Act
        var result = await _medicoService.BuscarMedicos(especialidade);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(medicosDto, result.Data);
    }

    [Fact]
    public async Task BuscarMedicos_DeveRetornarErro_QuandoOcorreExcecao()
    {
        // Arrange
        eEspecialidade? especialidade = null;
        var exception = new Exception("Erro ao buscar medicos");

        _medicoRepositoryMock
            .Setup(r => r.BuscarMedicos(especialidade))
            .ThrowsAsync(exception);

        // Act
        var result = await _medicoService.BuscarMedicos(especialidade);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(exception, result.Error);
    }
}
