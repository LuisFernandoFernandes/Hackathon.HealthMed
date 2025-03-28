using Hackathon.HealthMed.Api.Controllers;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Result;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Hackathon.HealthMed.Tests.Unit.Presentation
{
    public class HorarioControllerTest
    {
        private readonly Mock<IHorarioService> _horarioServiceMock;
        private readonly HorarioController _horarioController;

        public HorarioControllerTest()
        {
            _horarioServiceMock = new Mock<IHorarioService>();
            _horarioController = new HorarioController(_horarioServiceMock.Object);
        }

        [Fact]
        public async Task CadastrarHorario_DeveRetornarOk_QuandoResultadoForSucesso()
        {
            // Arrange
            var cadastrarHorarioDto = new CadastrarHorarioDTO
            {
                DataHorario = DateTime.UtcNow.AddDays(1),
                Valor = 150.00M,
            };

            var expectedGuid = Guid.NewGuid();
            // Cria um ServiceResult com um Guid de sucesso
            var serviceResult = new ServiceResult<Guid>(expectedGuid);
            _horarioServiceMock
                .Setup(s => s.CadastrarHorario(cadastrarHorarioDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.CadastrarHorario(cadastrarHorarioDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(expectedGuid, data);
        }

        [Fact]
        public async Task CadastrarHorario_DeveRetornarBadRequest_QuandoResultadoForFalha()
        {
            // Arrange
            var cadastrarHorarioDto = new CadastrarHorarioDTO
            {
                DataHorario = DateTime.UtcNow.AddDays(1),
                Valor = 150.00M,
            };

            var errorMessage = "Erro ao cadastrar horário";
            var serviceResult = new ServiceResult<Guid>(new Exception(errorMessage));
            _horarioServiceMock
                .Setup(s => s.CadastrarHorario(cadastrarHorarioDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.CadastrarHorario(cadastrarHorarioDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exception = Assert.IsType<Exception>(badRequestResult.Value);
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task EditarHorario_DeveRetornarOk_QuandoResultadoForSucesso()
        {
            // Arrange
            var editarHorarioDto = new EditarHorarioDTO
            {
                Id = Guid.NewGuid(),
                DataHorario = DateTime.UtcNow.AddDays(2),
                Valor = 200.00M,
            };

            var serviceResult = new ServiceResult<bool>(true);
            _horarioServiceMock
                .Setup(s => s.EditarHorario(editarHorarioDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.EditarHorario(editarHorarioDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<bool>(okResult.Value);
            Assert.True(data);
        }

        [Fact]
        public async Task EditarHorario_DeveRetornarBadRequest_QuandoResultadoForFalha()
        {
            // Arrange
            var editarHorarioDto = new EditarHorarioDTO
            {
                Id = Guid.NewGuid(),
                DataHorario = DateTime.UtcNow.AddDays(2),
                Valor = 200.00M,
            };

            var errorMessage = "Erro ao editar horário";
            var serviceResult = new ServiceResult<bool>(new Exception(errorMessage));
            _horarioServiceMock
                .Setup(s => s.EditarHorario(editarHorarioDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.EditarHorario(editarHorarioDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exception = Assert.IsType<Exception>(badRequestResult.Value);
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task BuscarMeusHorarios_DeveRetornarOk_QuandoResultadoForSucesso()
        {
            // Arrange
            var horariosEsperados = new List<HorarioDTO>
            {
                new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(1), Valor = 150.00M },
                new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(2), Valor = 200.00M }
            };

            var serviceResult = new ServiceResult<IEnumerable<HorarioDTO>>(horariosEsperados);
            _horarioServiceMock
                .Setup(s => s.BuscarMeusHorarios())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.BuscarMeusHorarios();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<HorarioDTO>>(okResult.Value);
            Assert.Equal(horariosEsperados, data);
        }

        [Fact]
        public async Task BuscarMeusHorarios_DeveRetornarBadRequest_QuandoResultadoForFalha()
        {
            // Arrange
            var errorMessage = "Erro ao buscar horários";
            var serviceResult = new ServiceResult<IEnumerable<HorarioDTO>>(new Exception(errorMessage));
            _horarioServiceMock
                .Setup(s => s.BuscarMeusHorarios())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.BuscarMeusHorarios();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exception = Assert.IsType<Exception>(badRequestResult.Value);
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task BuscarHorariosPorMedico_DeveRetornarOk_QuandoResultadoForSucesso()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var horariosEsperados = new List<HorarioDTO>
            {
                new HorarioDTO { Id = Guid.NewGuid(), DataHorario = DateTime.UtcNow.AddDays(1), Valor = 150.00M }
            };

            var serviceResult = new ServiceResult<IEnumerable<HorarioDTO>>(horariosEsperados);
            _horarioServiceMock
                .Setup(s => s.BuscarHorariosPorMedico(medicoId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.BuscarHorariosPorMedico(medicoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<HorarioDTO>>(okResult.Value);
            Assert.Equal(horariosEsperados, data);
        }

        [Fact]
        public async Task BuscarHorariosPorMedico_DeveRetornarBadRequest_QuandoResultadoForFalha()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var errorMessage = "Erro ao buscar horários do médico";
            var serviceResult = new ServiceResult<IEnumerable<HorarioDTO>>(new Exception(errorMessage));
            _horarioServiceMock
                .Setup(s => s.BuscarHorariosPorMedico(medicoId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _horarioController.BuscarHorariosPorMedico(medicoId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exception = Assert.IsType<Exception>(badRequestResult.Value);
            Assert.Equal(errorMessage, exception.Message);
        }
    }
}
