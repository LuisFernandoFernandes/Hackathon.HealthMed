using Hackathon.HealthMed.Api.Controllers;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Result;
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

        var expectedAgendamentoId = Guid.NewGuid();
        var serviceResult = new ServiceResult<Guid>(expectedAgendamentoId);
        _agendamentoServiceMock
            .Setup(s => s.AgendarConsulta(dto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _agendamentoController.AgendarConsulta(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(expectedAgendamentoId, data);
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
        var serviceResult = new ServiceResult<Guid>(new Exception(errorMessage));
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
}
