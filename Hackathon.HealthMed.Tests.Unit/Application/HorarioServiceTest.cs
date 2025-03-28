using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Services;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using StackExchange.Redis;
using System.Security.Claims;

namespace Hackathon.HealthMed.Tests.Unit.Application.Services;

public class HorarioServiceTest
{
    private readonly Mock<IHorarioRepository> _horarioRepositoryMock;
    private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly HorarioService _horarioService;

    private readonly Guid _medicoId;

    public HorarioServiceTest()
    {
        _horarioRepositoryMock = new Mock<IHorarioRepository>();
        _medicoRepositoryMock = new Mock<IMedicoRepository>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();

        _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                  .Returns(_redisDbMock.Object);

        _medicoId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _medicoId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        _medicoRepositoryMock.Setup(r => r.BuscarMedicoPorUsuarioId(_medicoId))
                             .ReturnsAsync(_medicoId);

        _horarioService = new HorarioService(
            _horarioRepositoryMock.Object,
            _medicoRepositoryMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object,
            _redisMock.Object
        );
    }

    #region CadastrarHorario

    [Fact]
    public async Task CadastrarHorario_DeveRetornarGuid_QuandoSucesso()
    {
        // Arrange
        var horarioDto = new CadastrarHorarioDTO
        {
            DataHorario = DateTime.UtcNow.AddDays(1),
            Valor = 150.00M,
            MedicoId = null
        };

        // Simula que não existe horário para a data
        _horarioRepositoryMock
            .Setup(r => r.BuscarHorarioPorMedicoEData(_medicoId, horarioDto.DataHorario))
            .ReturnsAsync(false);

        var horario = new Horario(_medicoId, horarioDto.DataHorario, horarioDto.Valor);
        var horarioId = Guid.NewGuid();
        typeof(Horario).GetProperty("Id").SetValue(horario, horarioId);

        _mapperMock.Setup(m => m.Map<Horario>(horarioDto))
                   .Returns(horario);

        _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync(true);

        _horarioRepositoryMock.Setup(r => r.Cadastrar(It.IsAny<Horario>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _horarioService.CadastrarHorario(horarioDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(horarioId, result.Data);
    }

    [Fact]
    public async Task CadastrarHorario_DeveRetornarErro_QuandoMedicoIdDiferente()
    {
        // Arrange
        var outroMedicoId = Guid.NewGuid();
        var horarioDto = new CadastrarHorarioDTO
        {
            DataHorario = DateTime.UtcNow.AddDays(1),
            Valor = 150.00M,
            MedicoId = outroMedicoId
        };

        // Act
        var result = await _horarioService.CadastrarHorario(horarioDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Você não tem permissão para cadastrar horários para outro médico.", result.Error.Message);
    }

    [Fact]
    public async Task CadastrarHorario_DeveRetornarErro_QuandoHorarioJaCadastrado()
    {
        // Arrange
        var horarioDto = new CadastrarHorarioDTO
        {
            DataHorario = DateTime.UtcNow.AddDays(1),
            Valor = 150.00M,
            MedicoId = null
        };

        _horarioRepositoryMock.Setup(r => r.BuscarHorarioPorMedicoEData(_medicoId, horarioDto.DataHorario))
                               .ReturnsAsync(true);

        // Act
        var result = await _horarioService.CadastrarHorario(horarioDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Horário já cadastrado para essa data/hora.", result.Error.Message);
    }

    #endregion

    #region EditarHorario

    [Fact]
    public async Task EditarHorario_DeveRetornarTrue_QuandoSucesso()
    {
        // Arrange
        var editarDto = new EditarHorarioDTO
        {
            Id = Guid.NewGuid(),
            DataHorario = DateTime.UtcNow.AddDays(2),
            Valor = 200.00M
        };

        var horario = new Horario(_medicoId, editarDto.DataHorario, editarDto.Valor);
        typeof(Horario).GetProperty("Id").SetValue(horario, editarDto.Id);

        var horariosList = new List<Horario> { horario };

        _horarioRepositoryMock.Setup(r => r.BuscarHorarioParaEdicao(_medicoId, editarDto.Id, editarDto.DataHorario))
                              .ReturnsAsync(horariosList);

        _mapperMock.Setup(m => m.Map(editarDto, horario))
                   .Verifiable();

        _horarioRepositoryMock.Setup(r => r.Editar(horario))
                              .Returns(Task.CompletedTask);

        _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync(true);

        // Act
        var result = await _horarioService.EditarHorario(editarDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        _mapperMock.Verify(m => m.Map(editarDto, horario), Times.Once);
    }

    [Fact]
    public async Task EditarHorario_DeveRetornarErro_QuandoHorarioNaoEncontrado()
    {
        // Arrange
        var editarDto = new EditarHorarioDTO
        {
            Id = Guid.NewGuid(),
            DataHorario = DateTime.UtcNow.AddDays(2),
            Valor = 200.00M
        };

        _horarioRepositoryMock.Setup(r => r.BuscarHorarioParaEdicao(_medicoId, editarDto.Id, editarDto.DataHorario))
                      .ReturnsAsync([]);


        // Act
        var result = await _horarioService.EditarHorario(editarDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Horário não encontrado ou não pertence ao médico logado.", result.Error.Message);
    }

    [Fact]
    public async Task EditarHorario_DeveRetornarErro_QuandoHorarioDuplicadoExistir()
    {
        // Arrange
        var editarDto = new EditarHorarioDTO
        {
            Id = Guid.NewGuid(),
            DataHorario = DateTime.UtcNow.AddDays(2),
            Valor = 200.00M
        };

        var horarioOriginal = new Horario(_medicoId, editarDto.DataHorario, editarDto.Valor);
        typeof(Horario).GetProperty("Id").SetValue(horarioOriginal, editarDto.Id);

        var horarioDuplicado = new Horario(_medicoId, editarDto.DataHorario, editarDto.Valor);
        typeof(Horario).GetProperty("Id").SetValue(horarioDuplicado, Guid.NewGuid());

        var horariosList = new List<Horario> { horarioOriginal, horarioDuplicado };

        _horarioRepositoryMock.Setup(r => r.BuscarHorarioParaEdicao(_medicoId, editarDto.Id, editarDto.DataHorario))
                              .ReturnsAsync(horariosList);

        // Act
        var result = await _horarioService.EditarHorario(editarDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Já existe um horário cadastrado para essa data/hora.", result.Error.Message);
    }

    #endregion

    #region BuscarHorarios

    [Fact]
    public async Task BuscarMeusHorarios_DeveRetornarHorariosDTO_QuandoSucesso()
    {
        // Arrange
        var horarios = new List<Horario>
        {
            new Horario(_medicoId, DateTime.UtcNow.AddDays(1), 150.00M),
            new Horario(_medicoId, DateTime.UtcNow.AddDays(2), 200.00M)
        };

        var horariosDto = new List<HorarioDTO>
        {
            new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(1), Status =                eStatusHorario.Disponivel, Valor = 150.00M },
            new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(2), Status = eStatusHorario.Disponivel, Valor = 200.00M }
        };


        _horarioRepositoryMock.Setup(r => r.BuscarHorarios(_medicoId, null))
                              .ReturnsAsync(horarios);

        _mapperMock.Setup(m => m.Map<IEnumerable<HorarioDTO>>(horarios))
                   .Returns(horariosDto);

        // Act
        var result = await _horarioService.BuscarMeusHorarios();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(horariosDto, result.Data);
    }

    [Fact]
    public async Task BuscarHorariosPorMedico_DeveRetornarHorariosDTO_QuandoSucesso()
    {
        // Arrange
        var medicoId = Guid.NewGuid();
        var horarios = new List<Horario>
        {
            new Horario(medicoId, DateTime.UtcNow.AddDays(1), 150.00M)
        };

        var horariosDto = new List<HorarioDTO>
        {
            new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(1), Status =        eStatusHorario.Disponivel, Valor = 150.00M }
        };


        _horarioRepositoryMock.Setup(r => r.BuscarHorarios(medicoId, eStatusHorario.Disponivel))
                              .ReturnsAsync(horarios);

        _mapperMock.Setup(m => m.Map<IEnumerable<HorarioDTO>>(horarios))
                   .Returns(horariosDto);

        // Act
        var result = await _horarioService.BuscarHorariosPorMedico(medicoId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(horariosDto, result.Data);
    }

    #endregion
}
