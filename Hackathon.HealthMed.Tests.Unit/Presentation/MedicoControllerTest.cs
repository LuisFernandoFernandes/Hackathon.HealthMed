using Hackathon.HealthMed.Api.Controllers;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Hackathon.HealthMed.Tests.Unit.Presentation
{
    public class MedicoControllerTest
    {
        private readonly Mock<IMedicoService> _medicoServiceMock;
        private readonly MedicoController _medicoController;

        public MedicoControllerTest()
        {
            _medicoServiceMock = new Mock<IMedicoService>();
            _medicoController = new MedicoController(_medicoServiceMock.Object);
        }

        [Fact]
        public async Task GetMedicos_DeveRetornarOk_QuandoResultadoForSucesso()
        {
            // Arrange
            eEspecialidade? especialidade = eEspecialidade.Cardiologia;
            IEnumerable<MedicoDTO> medicosList = [new MedicoDTO { Nome = "Dr. Teste", CRM = "12345" }];

            // Cria um ServiceResult usando IEnumerable<MedicoDTO>
            var serviceResult = new ServiceResult<IEnumerable<MedicoDTO>>(medicosList);

            _medicoServiceMock
                .Setup(s => s.BuscarMedicos(especialidade))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _medicoController.GetMedicos(especialidade);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<MedicoDTO>>(okResult.Value);
            Assert.Equal(medicosList, data);
        }

        [Fact]
        public async Task GetMedicos_DeveRetornarBadRequest_QuandoResultadoForFalha()
        {
            // Arrange
            eEspecialidade? especialidade = null;
            var errorMessage = "Erro ao buscar médicos";
            var serviceResult = new ServiceResult<IEnumerable<MedicoDTO>>(new System.Exception(errorMessage));

            var medicoServiceMock = new Mock<IMedicoService>();
            medicoServiceMock
                .Setup(s => s.BuscarMedicos(especialidade))
                .ReturnsAsync(serviceResult);

            var medicoController = new MedicoController(medicoServiceMock.Object);

            // Act
            var result = await medicoController.GetMedicos(especialidade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exception = Assert.IsType<System.Exception>(badRequestResult.Value);
            Assert.Equal(errorMessage, exception.Message);
        }
    }
}
