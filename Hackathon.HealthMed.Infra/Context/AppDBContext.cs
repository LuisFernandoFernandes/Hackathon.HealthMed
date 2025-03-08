using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;


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
}
