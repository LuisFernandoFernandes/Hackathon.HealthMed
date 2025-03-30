using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hackathon.HealthMed.Infra.Context;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDBContext).Assembly);
    }

    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCriacao") != null))
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property("DataCriacao").IsModified = false;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Método para inserir os dados iniciais no banco de dados.
    /// </summary>
    public async Task SeedData()
    {
        try
        {
            // Se não existir nenhum usuário, insere os dados iniciais
            if (!Usuarios.Any())
            {
                // Criando usuários iniciais (Médicos e Pacientes) com GUIDs fixos
                var usuarios = new List<Usuario>
                {
                    new Usuario("João Médico", "joao@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        DataCriacao = new DateTime(2024, 01, 01)
                    },
                    new Usuario("Maria Paciente", "maria@paciente.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Paciente)
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        DataCriacao = new DateTime(2024, 01, 02)
                    },
                    new Usuario("Carlos Paciente", "carlos@paciente.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Paciente)
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        DataCriacao = new DateTime(2024, 01, 03)
                    },
                    new Usuario("Ana Médica", "ana@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        DataCriacao = new DateTime(2024, 01, 04)
                    },
                    new Usuario("Pedro Médico", "pedro@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                        DataCriacao = new DateTime(2024, 01, 05)
                    },
                    new Usuario("Fernanda Médica", "fernanda@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                        DataCriacao = new DateTime(2024, 01, 06)
                    },
                    new Usuario("Ricardo Médico", "ricardo@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                        DataCriacao = new DateTime(2024, 01, 07)
                    },
                    new Usuario("Beatriz Médica", "beatriz@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                        DataCriacao = new DateTime(2024, 01, 08)
                    },
                    new Usuario("Gustavo Médico", "gustavo@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                        DataCriacao = new DateTime(2024, 01, 09)
                    },
                    new Usuario("Sofia Médica", "sofia@medico.com", BCrypt.Net.BCrypt.HashPassword("123456"), eTipoUsuario.Medico)
                    {
                        Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        DataCriacao = new DateTime(2024, 01, 10)
                    }
                };

                await Usuarios.AddRangeAsync(usuarios);

                // Criando médicos associados aos usuários médicos
                var medicos = new List<Medico>
                {
                    new Medico(usuarios[0].Id, "123456", eEspecialidade.Cardiologia)
                    {
                        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        DataCriacao = new DateTime(2024, 01, 11)
                    },
                    new Medico(usuarios[3].Id, "654321", eEspecialidade.Pediatria)
                    {
                        Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                        DataCriacao = new DateTime(2024, 01, 12)
                    },
                    new Medico(usuarios[4].Id, "987654", eEspecialidade.OrtopediaTraumatologia)
                    {
                        Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                        DataCriacao = new DateTime(2024, 01, 13)
                    },
                    new Medico(usuarios[5].Id, "456789", eEspecialidade.Cardiologia)
                    {
                        Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                        DataCriacao = new DateTime(2024, 01, 14)
                    },
                    new Medico(usuarios[6].Id, "112233", eEspecialidade.Dermatologia)
                    {
                        Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                        DataCriacao = new DateTime(2024, 01, 15)
                    },
                    new Medico(usuarios[7].Id, "223344", eEspecialidade.Neurologia)
                    {
                        Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                        DataCriacao = new DateTime(2024, 01, 16)
                    },
                    new Medico(usuarios[8].Id, "334455", eEspecialidade.Cardiologia)
                    {
                        Id = Guid.Parse("66666666-7777-8888-9999-000000000000"),
                        DataCriacao = new DateTime(2024, 01, 17)
                    },
                    new Medico(usuarios[9].Id, "445566", eEspecialidade.Urologia)
                    {
                        Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                        DataCriacao = new DateTime(2024, 01, 18)
                    }
                };

                await Medicos.AddRangeAsync(medicos);

                // Criando pacientes associados aos usuários pacientes
                var pacientes = new List<Paciente>
                {
                    new Paciente(usuarios[1].Id, "12345678900")
                    {
                        Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"),
                        DataCriacao = new DateTime(2024, 01, 19)
                    },
                    new Paciente(usuarios[2].Id, "09876543211")
                    {
                        Id = Guid.Parse("11112222-3333-4444-5555-666677778888"),
                        DataCriacao = new DateTime(2024, 01, 20)
                    }
                };

                await Pacientes.AddRangeAsync(pacientes);

                await SaveChangesAsync();
            }
        }
        catch
        {
        }
    }
}
