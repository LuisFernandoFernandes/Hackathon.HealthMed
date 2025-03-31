using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Repository;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Hackathon.HealthMed.Tests.Integration.Infra
{
    [Collection(nameof(ContextDbCollection))]
    [Order(5)]
    public class HorarioRepositoryTest
    {
        private readonly AppDBContext _context;
        private readonly IHorarioRepository _repository;

        public HorarioRepositoryTest(ContextDbFixture fixture)
        {
            _context = fixture.Context!;
            _repository = new HorarioRepository(_context);
        }

        /// <summary>
        /// Limpa a tabela de Horarios.
        /// </summary>
        private async Task ClearHorariosAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Agendamentos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Horarios");
        }

        /// <summary>
        /// Cria um usuário e um médico, retornando o MedicoId.
        /// </summary>
        private async Task<Guid> SeedMedicoAsync()
        {
            var usuario = new Usuario("Medico Teste", "medico@example.com", "hashSenha", eTipoUsuario.Medico);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var medico = new Medico(usuario.Id, "CRM12345", eEspecialidade.Cardiologia);
            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();

            return medico.Id;
        }

        [Fact]
        public async Task Cadastrar_DeveInserirHorarioNoBanco()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);
            var horario = new Horario(medicoId, dataHorario, 150m);

            // Act
            await _repository.Cadastrar(horario);

            // Assert
            var inserted = await _context.Horarios.FindAsync(horario.Id);
            Assert.NotNull(inserted);
            Assert.Equal(medicoId, inserted.MedicoId);
            Assert.Equal(150m, inserted.Valor);
            Assert.Equal(dataHorario, inserted.DataHorario);
        }

        [Fact]
        public async Task Editar_DeveAtualizarHorarioNoBanco()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);
            var horario = new Horario(medicoId, dataHorario, 150m);
            await _repository.Cadastrar(horario);

            // Atualiza os valores usando reflection
            var newValor = 200m; // literal decimal
            var newDataHorario = dataHorario.AddHours(2);
            var valorProp = typeof(Horario)
                .GetProperty("Valor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            valorProp.SetValue(horario, newValor);
            var dataProp = typeof(Horario)
                .GetProperty("DataHorario", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            dataProp.SetValue(horario, newDataHorario);

            // Act
            await _repository.Editar(horario);

            // Assert
            var updated = await _context.Horarios.FindAsync(horario.Id);
            Assert.NotNull(updated);
            Assert.Equal(newValor, updated.Valor);
            Assert.Equal(newDataHorario, updated.DataHorario);
        }

        [Fact]
        public async Task BuscarHorarioPorMedicoEData_DeveRetornarTrueSeExiste()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);
            var horario = new Horario(medicoId, dataHorario, 150m);
            await _repository.Cadastrar(horario);

            // Act
            var exists = await _repository.BuscarHorarioPorMedicoEData(medicoId, dataHorario);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task BuscarHorarioPorMedicoEData_DeveRetornarFalseSeNaoExiste()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);

            // Act
            var exists = await _repository.BuscarHorarioPorMedicoEData(medicoId, dataHorario);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task BuscarHorarioParaEdicao_DeveRetornarHorarioDoRegistroAtual()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);

            // Como o índice único impede duplicidade, apenas um registro pode existir para um dado horário.
            var horario = new Horario(medicoId, dataHorario, 150m);
            await _repository.Cadastrar(horario);

            // Act: Buscar para edição, passando o id do registro e a data
            var result = await _repository.BuscarHorarioParaEdicao(medicoId, horario.Id, dataHorario);

            // Assert: Espera-se que o método retorne o próprio registro
            Assert.Single(result);
            Assert.Equal(horario.Id, result.First().Id);
        }

        [Fact]
        public async Task BuscarHorarios_DeveRetornarHorariosAPartirDeHoje()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();

            // Cria um horário para ontem, hoje e amanhã
            var horarioPassado = new Horario(medicoId, DateTime.UtcNow.AddDays(-1), 150m);
            var horarioHoje = new Horario(medicoId, DateTime.UtcNow.AddHours(1), 150m);
            var horarioAmanha = new Horario(medicoId, DateTime.UtcNow.AddDays(1), 150m);
            await _repository.Cadastrar(horarioPassado);
            await _repository.Cadastrar(horarioHoje);
            await _repository.Cadastrar(horarioAmanha);

            // Act
            var result = await _repository.BuscarHorarios(medicoId, null);

            // Assert: Apenas horários de hoje em diante devem ser retornados
            Assert.DoesNotContain(result, h => h.Id == horarioPassado.Id);
            Assert.Contains(result, h => h.Id == horarioHoje.Id);
            Assert.Contains(result, h => h.Id == horarioAmanha.Id);
        }

        [Fact]
        public async Task BuscarHorarioPorIdEStatus_DeveRetornarHorario_QuandoStatusCorreto()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);
            var horario = new Horario(medicoId, dataHorario, 150m);
            // Atualiza o status para Reservado
            horario.AtualizarStatus(eStatusHorario.Reservado);
            await _repository.Cadastrar(horario);

            // Act
            var result = await _repository.BuscarHorarioPorIdEStatus(horario.Id, eStatusHorario.Reservado);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(horario.Id, result.Id);
        }

        [Fact]
        public async Task BuscarHorarioPorIdEStatus_DeveRetornarNull_QuandoStatusDiferente()
        {
            // Arrange
            await ClearHorariosAsync();
            var medicoId = await SeedMedicoAsync();
            var dataHorario = DateTime.UtcNow.AddDays(1);
            var horario = new Horario(medicoId, dataHorario, 150m);
            // Atualiza o status para Disponível
            horario.AtualizarStatus(eStatusHorario.Disponivel);
            await _repository.Cadastrar(horario);

            // Act
            var result = await _repository.BuscarHorarioPorIdEStatus(horario.Id, eStatusHorario.Reservado);

            // Assert
            Assert.Null(result);
        }
    }
}
