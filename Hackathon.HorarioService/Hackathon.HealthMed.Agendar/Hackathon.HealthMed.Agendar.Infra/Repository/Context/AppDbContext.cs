using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Agendar.Infra.Repository.Context;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Agendamento> Agendamentos { get; set; }
}
