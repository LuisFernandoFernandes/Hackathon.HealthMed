using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Hackathon.HealthMed.Tests.Integration.Fixture
{
    [CollectionDefinition(nameof(ContextDbCollection))]
    public class ContextDbCollection : ICollectionFixture<ContextDbFixture>
    {
    }

    public class ContextDbFixture : IAsyncLifetime
    {
        public AppDBContext? Context { get; private set; }
        public string sqlConnection { get; private set; } = "";

        // Configura o container do SQL Server
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(1434, true) // Porta alterada para 1434
            .Build();

        public async Task InitializeAsync()
        {
            // Inicia o container e configura o DbContext
            await _msSqlContainer.StartAsync();
            sqlConnection = _msSqlContainer.GetConnectionString().Replace("Database=master", "Database=HackathonHealthMedTest");


            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlServer(sqlConnection)
                .Options;

            Context = new AppDBContext(options);
            await Context.Database.MigrateAsync();
        }

        // Método para limpar (e opcionalmente semear) os dados das tabelas reais da aplicação
        public void InicializaDados()
        {
            // Excluir os dados na ordem correta para evitar conflitos de chave estrangeira:
            // 1. Agendamentos (filha de HorariosDisponiveis e Pacientes)
            // 2. HorariosDisponiveis (depende de Medicos)
            // 3. Medicos e Pacientes (dependem de Usuarios)
            // 4. Usuarios
            Context!.Database.ExecuteSqlRaw("DELETE FROM Agendamentos");
            Context.Database.ExecuteSqlRaw("DELETE FROM HorariosDisponiveis");
            Context.Database.ExecuteSqlRaw("DELETE FROM Medicos");
            Context.Database.ExecuteSqlRaw("DELETE FROM Pacientes");
            Context.Database.ExecuteSqlRaw("DELETE FROM Usuarios");

            // Opcional: Inserir dados de teste

            // Exemplo para um usuário do tipo Paciente
            var usuarioPaciente = new Usuario("Paciente Teste", "paciente@exemplo.com", BCrypt.Net.BCrypt.HashPassword("senha123"), eTipoUsuario.Paciente);
            Context.Usuarios.Add(usuarioPaciente);
            Context.SaveChanges();

            var paciente = new Paciente(usuarioPaciente.Id, "12345678901");
            Context.Pacientes.Add(paciente);
            Context.SaveChanges();

            // Exemplo para um usuário do tipo Médico
            var usuarioMedico = new Usuario("Medico Teste", "medico@exemplo.com", BCrypt.Net.BCrypt.HashPassword("senha123"), eTipoUsuario.Medico);
            Context.Usuarios.Add(usuarioMedico);
            Context.SaveChanges();

            var medico = new Medico(usuarioMedico.Id, "CRM123456", eEspecialidade.Cardiologia); // Ajuste a especialidade conforme necessário
            Context.Medicos.Add(medico);
            Context.SaveChanges();

        }

        public async Task DisposeAsync()
        {
            if (Context != null)
            {
                await Context.Database.EnsureDeletedAsync();
                Context.Dispose();
            }
            await _msSqlContainer.StopAsync();
        }
    }
}


//< PreserveCompilationContext > true </ PreserveCompilationContext >