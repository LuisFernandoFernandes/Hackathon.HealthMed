using Hackathon.HealthMed.Api.Controllers;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hackathon.HealthMed.Tests.Unit.Presentation;

public class AgendamentoControllerTest
{
    private readonly Mock<IAgendamentoService> _agendamentoServiceMock;
    private readonly AgendamentoController _agendamentoController;

    public AgendamentoControllerTest()
    {
        _agendamentoServiceMock = new Mock<IAgendamentoService>();
        _agendamentoController = new AgendamentoController(_agendamentoServiceMock.Object);
    }

    [Fact]
    public async Task AgendarConsulta_DeveRetornarOk_QuandoResultadoForSucesso()
    {
        // Arrange
        var dto = new AgendarConsultaDTO
        {
            HorarioId = Guid.NewGuid()
        };

        var expectedText = string.Empty;
        var serviceResult = new ServiceResult<string>(expectedText);
        _agendamentoServiceMock
            .Setup(s => s.AgendarConsulta(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.AgendarConsulta(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedText, data);
    }

    [Fact]
    public async Task AgendarConsulta_DeveRetornarBadRequest_QuandoResultadoForFalha()
    {
        // Arrange
        var dto = new AgendarConsultaDTO
        {
            HorarioId = Guid.NewGuid()
        };

        var errorMessage = "Horário inválido ou já reservado.";
        var serviceResult = new ServiceResult<string>(new Exception(errorMessage));
        _agendamentoServiceMock
            .Setup(s => s.AgendarConsulta(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.AgendarConsulta(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var exception = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveRetornarOk_QuandoResultadoForSucesso()
    {
        // Arrange
        var dto = new ConfirmarAgendamentoDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Aceitar = true
        };

        var serviceResult = new ServiceResult<bool>(true);
        _agendamentoServiceMock
            .Setup(s => s.ConfirmarAgendamento(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.AceitarOuRecusarConsulta(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<bool>(okResult.Value);
        Assert.True(data);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveRetornarBadRequest_QuandoResultadoForFalha()
    {
        // Arrange
        var dto = new ConfirmarAgendamentoDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Aceitar = false,
            Justificativa = "Horário indisponível"
        };

        var errorMessage = "Agendamento não encontrado.";
        var serviceResult = new ServiceResult<bool>(new Exception(errorMessage));
        _agendamentoServiceMock
            .Setup(s => s.ConfirmarAgendamento(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.AceitarOuRecusarConsulta(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var exception = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public async Task CancelarConsultaPaciente_DeveRetornarOk_QuandoResultadoForSucesso()
    {
        // Arrange
        var dto = new CancelarAgendamentoPacienteDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Justificativa = "Paciente não pode comparecer"
        };

        var serviceResult = new ServiceResult<bool>(true);
        _agendamentoServiceMock
            .Setup(s => s.CancelarPorPaciente(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.CancelarConsultaPaciente(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<bool>(okResult.Value);
        Assert.True(data);
    }

    [Fact]
    public async Task CancelarConsultaPaciente_DeveRetornarBadRequest_QuandoResultadoForFalha()
    {
        // Arrange
        var dto = new CancelarAgendamentoPacienteDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Justificativa = ""
        };

        var errorMessage = "Justificativa é obrigatória para cancelamento.";
        var serviceResult = new ServiceResult<bool>(new Exception(errorMessage));
        _agendamentoServiceMock
            .Setup(s => s.CancelarPorPaciente(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.CancelarConsultaPaciente(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var exception = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public async Task CancelarConsultaMedico_DeveRetornarOk_QuandoResultadoForSucesso()
    {
        // Arrange
        var dto = new CancelarAgendamentoMedicoDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Justificativa = "Médico não pode atender"
        };

        var serviceResult = new ServiceResult<bool>(true);
        _agendamentoServiceMock
            .Setup(s => s.CancelarPorMedico(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.CancelarConsultaMedico(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<bool>(okResult.Value);
        Assert.True(data);
    }

    [Fact]
    public async Task CancelarConsultaMedico_DeveRetornarBadRequest_QuandoResultadoForFalha()
    {
        // Arrange
        var dto = new CancelarAgendamentoMedicoDTO
        {
            AgendamentoId = Guid.NewGuid(),
            Justificativa = "Ação não permitida"
        };

        var errorMessage = "Ação não permitida. O horário não está disponível para o médico.";
        var serviceResult = new ServiceResult<bool>(new Exception(errorMessage));
        _agendamentoServiceMock
            .Setup(s => s.CancelarPorMedico(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.CancelarConsultaMedico(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var exception = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public async Task ConsultarAgendamentos_DeveRetornarOk_QuandoResultadoForSucesso()
    {
        // Arrange
        var agendamentos = new List<AgendamentoDTO>
    {
        new AgendamentoDTO
        {
            Id = Guid.NewGuid(),
            PacienteId = Guid.NewGuid(),
            HorarioId = Guid.NewGuid(),
            Status = eStatusAgendamento.Pendente,
            JustificativaCancelamento = null
        }
    };
        var serviceResult = new ServiceResult<IEnumerable<AgendamentoDTO>>(agendamentos);
        _agendamentoServiceMock
            .Setup(s => s.ConsultarAgendamentos())
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.ConsultarAgendamentos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<AgendamentoDTO>>(okResult.Value);
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task ConsultarAgendamentos_DeveRetornarBadRequest_QuandoResultadoForFalha()
    {
        // Arrange
        var errorMessage = "Erro ao consultar agendamentos.";
        var serviceResult = new ServiceResult<IEnumerable<AgendamentoDTO>>(new Exception(errorMessage));
        _agendamentoServiceMock
            .Setup(s => s.ConsultarAgendamentos())
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.ConsultarAgendamentos();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var exception = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Equal(errorMessage, exception.Message);
    }

}
