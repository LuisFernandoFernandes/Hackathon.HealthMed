using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Application.Services;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Hackathon.HealthMed.Tests.Unit.Application.Services
{
    public class AgendamentoServiceTest
    {
        private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly Mock<IHorarioRepository> _horarioRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _redisDbMock;
        private readonly AgendamentoService _agendamentoService;

        private readonly Guid _usuarioId;
        private readonly Guid _pacienteId;
        private readonly Guid _medicoId;

        public AgendamentoServiceTest()
        {
            _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
            _medicoRepositoryMock = new Mock<IMedicoRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _horarioRepositoryMock = new Mock<IHorarioRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _redisMock = new Mock<IConnectionMultiplexer>();
            _redisDbMock = new Mock<IDatabase>();

            _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                      .Returns(_redisDbMock.Object);

            // Configura o usuário autenticado
            _usuarioId = Guid.NewGuid();
            _pacienteId = Guid.NewGuid();
            _medicoId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _usuarioId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

            _pacienteRepositoryMock.Setup(r => r.BuscarPacientePorUsuarioId(_usuarioId))
                                   .ReturnsAsync(_pacienteId);
            _medicoRepositoryMock.Setup(r => r.BuscarMedicoPorUsuarioId(_usuarioId))
                                 .ReturnsAsync(_medicoId);

            _agendamentoService = new AgendamentoService(
                _agendamentoRepositoryMock.Object,
                _medicoRepositoryMock.Object,
                _pacienteRepositoryMock.Object,
                _horarioRepositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _redisMock.Object
            );
        }

        #region AgendarConsulta

        [Fact]
        public async Task AgendarConsulta_DeveRetornarGuid_QuandoSucesso()
        {
            // Arrange
            var dto = new AgendarConsultaDTO
            {
                HorarioId = Guid.NewGuid()
            };

            var horario = new Horario(_medicoId, DateTime.UtcNow.AddDays(1), 150.00M);
            Agendamento capturedAgendamento = null;
            _horarioRepositoryMock
                .Setup(r => r.BuscarHorarioPorIdEStatus(dto.HorarioId, eStatusHorario.Disponivel))
                .ReturnsAsync(horario);

            _agendamentoRepositoryMock
                .Setup(r => r.Adicionar(It.IsAny<Agendamento>()))
                .Callback<Agendamento>(ag =>
                {
                    capturedAgendamento = ag;
                    var newId = Guid.NewGuid();
                    typeof(Agendamento).GetProperty("Id").SetValue(ag, newId);
                })
                .Returns(Task.CompletedTask);

            _horarioRepositoryMock.Setup(r => r.Editar(It.IsAny<Horario>()))
                                  .Returns(Task.CompletedTask);
            _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _agendamentoService.AgendarConsulta(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(capturedAgendamento);
            var expectedId = (Guid)typeof(Agendamento).GetProperty("Id").GetValue(capturedAgendamento);
            Assert.Equal(expectedId, result.Data);
        }

        [Fact]
        public async Task AgendarConsulta_DeveRetornarErro_QuandoHorarioNaoDisponivel()
        {
            // Arrange
            var dto = new AgendarConsultaDTO
            {
                HorarioId = Guid.NewGuid()
            };

            // Simula que o horário não está disponível (null)
            _horarioRepositoryMock
                .Setup(r => r.BuscarHorarioPorIdEStatus(dto.HorarioId, eStatusHorario.Disponivel))
                .ReturnsAsync((Horario)null);

            // Act
            var result = await _agendamentoService.AgendarConsulta(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Horário inválido ou já reservado.", result.Error.Message);
        }

        #endregion

        #region ConfirmarAgendamento

        [Fact]
        public async Task ConfirmarAgendamento_DeveRetornarTrue_QuandoAceitarConsulta()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var dto = new ConfirmarAgendamentoDTO
            {
                AgendamentoId = agendamentoId,
                Aceitar = true
            };

            // Cria um agendamento simulado
            var agendamento = new Agendamento(_pacienteId, Guid.NewGuid());
            typeof(Agendamento).GetProperty("Id").SetValue(agendamento, agendamentoId);

            // Horário associado com status Reservado e com o mesmo médico autenticado
            var horario = new Horario(_medicoId, DateTime.UtcNow.AddDays(1), 150.00M);

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(agendamentoId))
                                      .ReturnsAsync(agendamento);
            _horarioRepositoryMock.Setup(r => r.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado))
                                  .ReturnsAsync(horario);
            _horarioRepositoryMock.Setup(r => r.Editar(It.IsAny<Horario>()))
                                  .Returns(Task.CompletedTask);
            _agendamentoRepositoryMock.Setup(r => r.Editar(It.IsAny<Agendamento>()))
                                      .Returns(Task.CompletedTask);
            _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _agendamentoService.ConfirmarAgendamento(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task ConfirmarAgendamento_DeveRetornarErro_QuandoAgendamentoNaoEncontrado()
        {
            // Arrange
            var dto = new ConfirmarAgendamentoDTO
            {
                AgendamentoId = Guid.NewGuid(),
                Aceitar = true
            };

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(dto.AgendamentoId))
                                      .ReturnsAsync((Agendamento)null);

            // Act
            var result = await _agendamentoService.ConfirmarAgendamento(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Agendamento não encontrado.", result.Error.Message);
        }

        [Fact]
        public async Task ConfirmarAgendamento_DeveRetornarErro_QuandoHorarioNaoPertenceAoMedico()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var dto = new ConfirmarAgendamentoDTO
            {
                AgendamentoId = agendamentoId,
                Aceitar = true
            };

            var agendamento = new Agendamento(_pacienteId, Guid.NewGuid());
            typeof(Agendamento).GetProperty("Id").SetValue(agendamento, agendamentoId);

            // Horário com MedicoId diferente do autenticado
            var horario = new Horario(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 150.00M);

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(agendamentoId))
                                      .ReturnsAsync(agendamento);
            _horarioRepositoryMock.Setup(r => r.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado))
                                  .ReturnsAsync(horario);

            // Act
            var result = await _agendamentoService.ConfirmarAgendamento(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Ação não permitida. O horário não pertence ao médico.", result.Error.Message);
        }

        #endregion

        #region CancelarPorMedico

        [Fact]
        public async Task CancelarPorMedico_DeveRetornarTrue_QuandoSucesso()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var dto = new CancelarAgendamentoMedicoDTO
            {
                AgendamentoId = agendamentoId,
                Justificativa = "Médico não pode atender"
            };

            var agendamento = new Agendamento(_pacienteId, Guid.NewGuid());
            typeof(Agendamento).GetProperty("Id").SetValue(agendamento, agendamentoId);

            // Horário com status Reservado e com o mesmo médico autenticado
            var horario = new Horario(_medicoId, DateTime.UtcNow.AddDays(1), 150.00M);

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(agendamentoId))
                                      .ReturnsAsync(agendamento);
            _horarioRepositoryMock.Setup(r => r.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado))
                                  .ReturnsAsync(horario);
            _horarioRepositoryMock.Setup(r => r.Editar(It.IsAny<Horario>()))
                                  .Returns(Task.CompletedTask);
            _agendamentoRepositoryMock.Setup(r => r.Editar(It.IsAny<Agendamento>()))
                                      .Returns(Task.CompletedTask);
            _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _agendamentoService.CancelarPorMedico(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CancelarPorMedico_DeveRetornarErro_QuandoAgendamentoNaoEncontrado()
        {
            // Arrange
            var dto = new CancelarAgendamentoMedicoDTO
            {
                AgendamentoId = Guid.NewGuid(),
                Justificativa = "Erro"
            };

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(dto.AgendamentoId))
                                      .ReturnsAsync((Agendamento)null);

            // Act
            var result = await _agendamentoService.CancelarPorMedico(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Agendamento não encontrado.", result.Error.Message);
        }

        #endregion

        #region CancelarPorPaciente

        [Fact]
        public async Task CancelarPorPaciente_DeveRetornarTrue_QuandoSucesso()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var dto = new CancelarAgendamentoPacienteDTO
            {
                AgendamentoId = agendamentoId,
                Justificativa = "Paciente não poderá comparecer"
            };

            // Cria um agendamento com status Agendado e PacienteId compatível
            var agendamento = new Agendamento(_pacienteId, Guid.NewGuid());
            typeof(Agendamento).GetProperty("Id").SetValue(agendamento, agendamentoId);
            // Ajusta o status para Agendado
            agendamento.AtualizarStatus(eStatusAgendamento.Agendado);

            // Horário associado (pode estar reservado)
            var horario = new Horario(_medicoId, DateTime.UtcNow.AddDays(1), 150.00M);
            // Não é obrigatório que o horário seja encontrado para cancelar, mas se encontrado, será atualizado

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(agendamentoId))
                                      .ReturnsAsync(agendamento);
            _pacienteRepositoryMock.Setup(r => r.BuscarPacientePorUsuarioId(_usuarioId))
                                   .ReturnsAsync(_pacienteId);
            _horarioRepositoryMock.Setup(r => r.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado))
                                  .ReturnsAsync(horario);
            _horarioRepositoryMock.Setup(r => r.Editar(It.IsAny<Horario>()))
                                  .Returns(Task.CompletedTask);
            _agendamentoRepositoryMock.Setup(r => r.Editar(It.IsAny<Agendamento>()))
                                      .Returns(Task.CompletedTask);
            _redisDbMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _agendamentoService.CancelarPorPaciente(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CancelarPorPaciente_DeveRetornarErro_QuandoJustificativaVazia()
        {
            // Arrange
            var dto = new CancelarAgendamentoPacienteDTO
            {
                AgendamentoId = Guid.NewGuid(),
                Justificativa = "" // Justificativa vazia
            };

            // Para que chegue até a validação da justificativa, precisamos simular um agendamento válido
            var agendamento = new Agendamento(_pacienteId, Guid.NewGuid());
            typeof(Agendamento).GetProperty("Id").SetValue(agendamento, dto.AgendamentoId);
            // Configura um status válido para cancelamento
            agendamento.AtualizarStatus(eStatusAgendamento.Agendado);

            _agendamentoRepositoryMock.Setup(r => r.BuscarPorId(dto.AgendamentoId))
                                      .ReturnsAsync(agendamento);
            _pacienteRepositoryMock.Setup(r => r.BuscarPacientePorUsuarioId(_usuarioId))
                                   .ReturnsAsync(_pacienteId);

            // Act & Assert
            var result = await _agendamentoService.CancelarPorPaciente(dto);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Justificativa é obrigatória para cancelamento.", result.Error.Message);
        }

        #endregion
    }
}
